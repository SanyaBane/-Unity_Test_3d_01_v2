using System;
using System.Collections.Generic;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.StateMachineScripts;
using System.Linq;
using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.AreaOfEffects;
using Assets.Scripts.Extensions;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class BaseCombatState : BaseNpcAiState
    {
        protected readonly NpcAI _npcAI;
        protected readonly AbilitiesController _abilitiesController;
        protected readonly AutoAttackController _autoAttackController;
        protected readonly INpcBaseCreature _npcBaseCreature;

        private const float ROTATION_DOT_MAX_VAL = 0.999f;

        public Vector3 CachedOnEnterPosition { get; private set; }
        public Quaternion CachedOnEnterRotation { get; private set; }

        // private float cachedSlowdownDistance;
        // private float cachedEndReachedDistance;
        // private bool cachedIsStopped;

        private ITargetable _previousTarget = null;

        public AggroController AggroController { get; }

        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Custom;

        public float GetEndReachedDistanceToTarget(IBaseCreature target)
        {
            var ret = _npcAI.INpcBaseCreature.AutoAttackController.AbilityAutoAttack.PreferableMaxDistance + _npcBaseCreature.CreatureMeasures.Radius + target.CreatureMeasures.Radius;
            return ret;
        }

        public float GetSlowdownDistanceToTarget(IBaseCreature target)
        {
            var ret = _npcAI.INpcBaseCreature.AutoAttackController.AbilityAutoAttack.Distance + _npcBaseCreature.CreatureMeasures.Radius + target.CreatureMeasures.Radius;
            return ret;
        }

        public bool IsOnAutoAttackDistance(IBaseCreature target)
        {
            var distanceCanAutoAttack = _npcAI.INpcBaseCreature.AutoAttackController.AbilityAutoAttack.Distance + _npcBaseCreature.CreatureMeasures.Radius + target.CreatureMeasures.Radius;
            var distanceToTargetSquared = VectorHelper.DistanceSquared(_npcBaseCreature.GetRootObjectTransform().transform.position, target.GetRootObjectTransform().transform.position);

            if (distanceCanAutoAttack * distanceCanAutoAttack < distanceToTargetSquared)
                return true;

            return false;
        }

        public BaseCombatState(NpcAI npcAI, INpcBaseCreature npcBaseCreature, Animator animator,
            AbilitiesController abilitiesController, AutoAttackController autoAttack) : base(npcAI)
        {
            _npcAI = npcAI;
            _npcBaseCreature = npcBaseCreature;
            _abilitiesController = abilitiesController;
            _autoAttackController = autoAttack;

            AggroController = new AggroController(npcAI);

            _abilitiesController.CastStarted += AbilitiesControllerOnCastStarted;

            _npcAI.NPCTargetHandler.SelectedTargetChanged += NPCTargetHandlerOnSelectedTargetChanged;
            _npcAI.CanMoveChanged += CanMoveChanged;
            _npcAI.CanMoveToTargetChanged += CanMoveToTargetChanged;
        }

        private void CanMoveChanged(bool obj)
        {
            CheckMove();
        }

        private void CanMoveToTargetChanged(bool obj)
        {
            CheckMove();
        }

        private void NPCTargetHandlerOnSelectedTargetChanged(ITargetable selectedTarget)
        {
            CheckMove();
        }

        private void CheckMove()
        {
            if (_npcAI.INpcBaseCreature.TargetHandler.SelectedTarget != null)
            {
                // _npcAI.AIDestinationSetter.target = _npcAI.INpcBaseCreature.TargetHandler.SelectedTarget.IBaseCreature.GetRootObjectTransform();
                if (_npcAI.AIPath != null)
                {
                    _npcAI.AIPath.destination = _npcAI.INpcBaseCreature.TargetHandler.SelectedTarget.IBaseCreature.GetGroundedPosition();
                }
            }

            // if (IsAllowedToMove())
            // {
            //     _npcAI.AIPath.destination = _npcAI.NPCTargetHandler.SelectedTarget.IBaseCreature.GetGroundedPosition();
            //     _npcAI.AIPath.SearchPath(); // todo maybe we should check if Creature is already looking for path, and in this case cancel previous searh, before starting a new one?
            // }
            // else
            // {
            //     _npcAI.AIPath.destination = Vector3.positiveInfinity;
            // }
        }

        private void ONSearchPath()
        {
            // Debug.Log($"ONSearchPath();");
            HandleMoving();
        }

        private void HandleSafePosition(Vector3 creaturePosition, IEnumerable<AOECircleController> listOfDangerousCircularAOE, Vector3 targetPosition, float radiusOffset)
        {
            creaturePosition = new Vector3(creaturePosition.x, 0, creaturePosition.z);
            targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

            Vector3? safePosition = AOECircleController.GetNearestPositionSafeFromAOE(_npcAI, creaturePosition, listOfDangerousCircularAOE, targetPosition, radiusOffset);
            if (safePosition != null)
            {
                _npcAI.IsEvadingAOE = true;
                _npcAI.AIPath.destination = safePosition.Value;

                _npcAI.AIPath.SetSlowdownDistance(_npcAI.EscapeFromAoeSlowdownDistance);
                _npcAI.AIPath.SetEndReachedDistance(_npcAI.EscapeFromAoeEndReachedDistance);
            }
            else
            {
                // if there is no safe position, just catch AoE. No choice =(
                Debug.Log($"Safe spot not found for creature '{_npcBaseCreature.GetRootObjectTransform().gameObject.name}'.");
                _npcAI.IsEvadingAOE = false;
            }
        }

        private void HandleMoving()
        {
            if (_npcAI.CombatTacticsAI.IsCombatTacticsHandleMoving)
            {
                return;
            }

            IBaseCreature selectedTargetCreature = null;

            try
            {
                selectedTargetCreature = _npcBaseCreature.NPCTargetHandler.SelectedTarget.IBaseCreature;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            if (IsAllowedToMove())
            {
                if (_npcAI.IsShouldEvadeAOE)
                {
                    List<AOECircleController> listAOEsCreatureIn = AOECircleController.GetAOEsCreatureIn(
                        _npcBaseCreature.GetGroundedPosition(),
                        GameManager.Instance.AOEManager.GetActiveAOECircleControllers(),
                        _npcAI.INpcBaseCreature.CreatureSO.CatchAOEByCapsuleCollider,
                        0);

                    // bool isCreatureInsideAOEs = IsCreatureInsideAOEs(
                    //     _npcBaseCreature.GetGroundedPosition(),
                    //     GameManager.Instance.AOEManager.AoeCircleControllers,
                    //     _npcAI.INpcBaseCreature.CreatureSO.CatchAOEByCapsuleCollider,
                    //     0);

                    // if (isCreatureInsideAOEs)
                    if (listAOEsCreatureIn.Count > 0)
                    {
                        var aoeGONames = String.Join(", ", listAOEsCreatureIn.Select(x => x.gameObject.name));
                        Debug.Log($"'{_npcBaseCreature.GetRootObjectTransform().gameObject.name}' is inside AOE(s): {aoeGONames}.");

                        HandleSafePosition(_npcBaseCreature.GetGroundedPosition(), GameManager.Instance.AOEManager.GetActiveAOECircleControllers(), selectedTargetCreature.GetGroundedPosition(), 0);
                    }
                    else
                    {
                        _npcAI.IsEvadingAOE = false;
                    }
                }

                if (!_npcAI.IsEvadingAOE)
                {
                    if (_npcAI.CanMoveToTarget)
                    {
                        float distanceToTarget = Vector3.Distance(_npcBaseCreature.GetGroundedPosition(), selectedTargetCreature.GetGroundedPosition());
                        // float distanceBetweenCreatureCollidersToTarget = TargetHelper.DistanceBetweenCreatureColliders(_npcBaseCreature, selectedTargetCreature);

                        if (_npcAI.IsShouldEvadeAOE)
                        {
                            bool isCreatureInsideAOEs = AOECircleController.IsCreatureInsideAOEs(
                                selectedTargetCreature.GetGroundedPosition(),
                                GameManager.Instance.AOEManager.GetActiveAOECircleControllers(),
                                _npcAI.INpcBaseCreature.CreatureSO.CatchAOEByCapsuleCollider,
                                _npcBaseCreature.AutoAttackController.AbilityAutoAttack.Distance);

                            if (isCreatureInsideAOEs == false)
                            {
                                // Debug.Log($"1. '{_npcBaseCreature.GetRootObjectTransform().gameObject.name}' moving to target '{selectedTargetCreature.GetRootObjectTransform().gameObject.name}'.");

                                MoveToTarget(selectedTargetCreature, distanceToTarget);
                            }
                            else
                            {
                                // if (IsOnAutoAttackDistance(selectedTargetCreature))
                                // {
                                //     MoveToTarget(selectedTargetCreature, distanceToTarget);
                                //     // _aiPath.destination = _aiPath.position;
                                //     // TryRotateToTarget();
                                //     // Debug.Log($"Test");
                                // }
                                // else
                                // {
                                // Debug.Log($"'{_npcBaseCreature.GetRootObjectTransform().gameObject.name}' moving to target  '{selectedTargetCreature.GetRootObjectTransform().gameObject.name}' nearest point outside of AOE(s).");

                                var radiusOffset = _npcBaseCreature.AutoAttackController.AbilityAutoAttack.Distance + _npcBaseCreature.CreatureMeasures.Radius + selectedTargetCreature.CreatureMeasures.Radius;
                                HandleSafePosition(_npcBaseCreature.GetGroundedPosition(), GameManager.Instance.AOEManager.GetActiveAOECircleControllers(), selectedTargetCreature.GetGroundedPosition(), radiusOffset);
                                // }
                            }
                        }
                        else
                        {
                            // if (IsOnAutoAttackDistance(selectedTargetCreature))
                            // {
                            //     MoveToTarget(selectedTargetCreature, distanceToTarget);
                            // }
                            // else
                            // {
                            //     Debug.Log($"2. '{_npcBaseCreature.GetRootObjectTransform().gameObject.name}' moving to target '{selectedTargetCreature.GetRootObjectTransform().gameObject.name}'.");
                            //
                            //     MoveToTarget(selectedTargetCreature, distanceToTarget);
                            // }

                            // Debug.Log($"2. '{_npcBaseCreature.GetRootObjectTransform().gameObject.name}' moving to target '{selectedTargetCreature.GetRootObjectTransform().gameObject.name}'.");
                            MoveToTarget(selectedTargetCreature, distanceToTarget);
                        }
                    }
                }

                if (_npcAI.AIPath != null && _npcAI.AIPath.reachedDestination)
                {
                    // TryRotateToTarget();
                }
            }
            else
            {
                // TryRotateToTarget();
            }
        }

        public override void TickState()
        {
            base.TickState();
            
            _npcAI.CombatTacticsAI.ResetCombatTactics();

            var primaryTarget = AggroController.GetPrimaryTarget();
            if (primaryTarget != null)
            {
                _npcAI.NPCTargetHandler.SelectTarget(primaryTarget.ITargetable);
            }

            if (_npcBaseCreature.NPCTargetHandler.SelectedTarget == null)
                return;

            IBaseCreature selectedTargetCreature = _npcBaseCreature.NPCTargetHandler.SelectedTarget.IBaseCreature;
            if (selectedTargetCreature == null)
            {
                _npcBaseCreature.CombatInfoHandler.DisengageCombatWithEveryone();
                return;
            }

            if (_autoAttackController != null && !_autoAttackController.AutoAttackModeOn)
                _autoAttackController.EnableAutoAttackMode();

            if (_npcAI.AIPath != null)
            {
                if (_npcAI.AIPath.reachedDestination || (_npcAI.AIPath.reachedDestination == false && IsAllowedToMove() == false))
                    TryRotateToTarget();
            }
            
            ExecuteSpecificBehaviour();
        }

        protected virtual void ExecuteSpecificBehaviour()
        {
            _npcAI.CombatTacticsAI.ProcessCombatTactics();
        }

        protected virtual void AbilitiesControllerOnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.CastTime > 0)
            {
                SetCastingInProgressState();
            }

            _abilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
            _abilitiesController.CastFinishedAndExecuted += AbilitiesControllerOnCastFinishedAndExecuted;
            _abilitiesController.CastInterrupted += AbilitiesControllerOnCastInterrupted;
        }

        protected virtual void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            _abilitiesController.CastFinished -= AbilitiesControllerOnCastFinished;
            _abilitiesController.CastInterrupted -= AbilitiesControllerOnCastInterrupted;
        }

        protected virtual void AbilitiesControllerOnCastInterrupted(AbilitiesController abilitiesController, Ability ability)
        {
            _abilitiesController.CastFinished -= AbilitiesControllerOnCastFinished;
            _abilitiesController.CastFinishedAndExecuted -= AbilitiesControllerOnCastFinishedAndExecuted;
            _abilitiesController.CastInterrupted -= AbilitiesControllerOnCastInterrupted;
        }

        protected virtual void AbilitiesControllerOnCastFinishedAndExecuted(AbilitiesController abilitiesController, Ability ability)
        {
            _abilitiesController.CastFinishedAndExecuted -= AbilitiesControllerOnCastFinishedAndExecuted;

            ResetCastingInProgressState();
        }

        private void SetCastingInProgressState()
        {
            // _npcAI.CastingAbility = true;

            // TODO Создать условие - если абилка может кастоваться только на месте
            // if (_aiPath != null && _npcAI._isStopMoveWhenCastingNonInstantAbility)
            // {
            //     _npcAI.CanMove = false;
            // }
        }

        private void ResetCastingInProgressState()
        {
            // if (_aiPath != null && _npcAI._isStopMoveWhenCastingNonInstantAbility)
            // {
            //     _npcAI.CanMove = true;
            // }

            // _npcAI.CastingAbility = false;
        }

        private void MoveToTarget(IBaseCreature selectedTargetCreature)
        {
            float distanceToTarget = Vector3.Distance(_npcBaseCreature.GetGroundedPosition(), selectedTargetCreature.GetGroundedPosition());
            MoveToTarget(selectedTargetCreature, distanceToTarget);
        }

        // TODO maybe we can use "DistanceSquared" to improve performance
        private void MoveToTarget(IBaseCreature selectedTargetCreature, float distanceToTarget)
        {
            if (_npcAI.AIPath == null)
            {
                Debug.LogError($"{nameof(_npcAI.AIPath)} is null.");
            }

            //_seeker.StartPath(_npcBaseCreature.GetTransform().position, targetGroundedPosition, OnPathComplete);
            _npcAI.AIPath.destination = selectedTargetCreature.GetGroundedPosition();

            _npcAI.AIPath.SetEndReachedDistance(GetEndReachedDistanceToTarget(selectedTargetCreature));
            _npcAI.AIPath.SetSlowdownDistance(GetSlowdownDistanceToTarget(selectedTargetCreature));

            //bool isSteeringTargetSameAsTarget = Vector3.Distance(targetPosition, _navMeshAgent.steeringTarget) < 0.05f;
            //Vector3 directionToTarget = targetGroundedPosition - _npcAI.transform.position;
            //Vector3 directionToSteeringTarget = _navMeshAgent.steeringTarget - _npcAI.transform.position;

            //Debug.DrawRay(_npcAI.transform.position + new Vector3(0, 1, 0), directionToTarget.normalized, Color.red);

            //var dotProductVelocityDesiredToActualVelocity = Vector3.Dot(_navMeshAgent.velocity.normalized, _navMeshAgent.desiredVelocity.normalized);

            // если существо стоит и не двигается, и при этом "SteeringTarget" отличается от "Target", значит возможен случай что
            // "SteeringTarget" показывает на точку в пространстве, а сама цель стоит вне досягаемости navMeshAgent, но всё ещё в поле зрения существа
            // а значит, поворачиваться надо к "Target", а не к "SteeringTarget" (к примеру если "Игрок" запрыгнул на высокое препятствие и передвинулся по нему).
            //float distanceToSteeringTarget = Vector3.Distance(_navMeshAgent.steeringTarget, _creatureSM.transform.position);
            //GameManager.Instance.GUIManager.UpdateUIVariables(8, $"_aiPath.velocity:\t{_aiPath.velocity}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(9, $"_aiPath.desiredVelocity:\t{_aiPath.desiredVelocity}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(10, $"_aiPath.reachedDestination:\t{_aiPath.reachedDestination}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(11, $"_aiPath.reachedEndOfPath:\t{_aiPath.reachedEndOfPath}");

            // если существо достигло цели, требуется вручную его поворачивать
            if (_npcAI.AIPath.reachedDestination)
            {
                TryRotateToTarget();
            }
            else
            {
                //RotateToDirection(directionToSteeringTarget);
            }

            //_npcAI.NodeGizmos.Clear();
            ////_nodes.Add()


            //NNInfo getNearest = AstarPath.active.GetNearest(_aiPath.destination);

            //if (getNearest.node.Graph is NavMeshGraph navMeshGraph)
            //{
            //    //navMeshGraph.GetTouchingTiles(Bounds)
            //}

            //NNInfo getNearest = GameManager.Instance.AstarPath.GetNearest(_aiPath.destination);


            //AstarPath.active.get

            //if (getNearest.node is TriangleMeshNode triangleMeshNode)
            //{
            //    foreach(var con in triangleMeshNode.connections)
            //    {
            //        _npcAI.NodeGizmos.Add((Vector3)con.node.position);
            //    }
            //}

            //Debug.Log("");

            //// если собирается куда-то двигаться
            //if (!_aiPath.isStopped)
            //{
            //    if (dotProductVelocityDesiredToActualVelocity > ROTATION_DOT_MAX_VAL)
            //    {
            //        // Если velocity соответствует desiredVelocity:
            //        // Существо смотрит в ту сторону, куда ему нужно

            //        ResetAccelerationToBaseValue();
            //    }
            //    else
            //    {
            //        // Если velocity не соответствует desiredVelocity:
            //        // Существо смотрит НЕ в ту сторону, куда ему нужно

            //        IncreaseAccelerationToHighValue();
            //    }
            //}
            //// Если существо и его navMesh остановлены, переключать Acceleration на базовое значение
            //else if (_aiPath.isStopped)
            //{
            //    ResetAccelerationToBaseValue();
            //}

            string testingMobGOName = "EnemyExample2B (12)";
            bool usingTestingMob = _npcBaseCreature.GetRootObjectTransform().gameObject.name == testingMobGOName;
            if (usingTestingMob)
            {
            }

            // Получаем список юнитов которые находятся в бою с целью.
            var othersEngagedToBattleWithTarget = selectedTargetCreature.CombatInfoHandler.GetEngagedCreatures().Where(x => x != _npcBaseCreature).ToList();
            if (distanceToTarget <= _autoAttackController.AbilityAutoAttackSO.Distance &&
                !IsAllowedToMove()) // если существо стоит на месте
            {
                if (othersEngagedToBattleWithTarget.Count() > 0)
                {
                    // Если существо стоит слишком близко к напарнику (который также атакует нашу цель),
                    // значит надо подвинуться в другом направлении от напарника (но чтобы одновременно не сильно отходить от цели).

                    // Находим ближайшего напарника.
                    IBaseCreature closestBuddy = othersEngagedToBattleWithTarget[0];

                    if (othersEngagedToBattleWithTarget.Count() > 1)
                    {
                        for (int i = 1; i < othersEngagedToBattleWithTarget.Count(); i++)
                        {
                            var iteratableBuddy = othersEngagedToBattleWithTarget[i];

                            Vector3 closestBuddyDistance = closestBuddy.GetRootObjectTransform().position - _npcBaseCreature.GetRootObjectTransform().position;
                            Vector3 iteratableBuddyDistance = iteratableBuddy.GetRootObjectTransform().position - _npcBaseCreature.GetRootObjectTransform().position;

                            // если нашли напарника ближе чем "текущий ближайший"
                            if (Vector3.SqrMagnitude(iteratableBuddyDistance) < Vector3.SqrMagnitude(closestBuddyDistance))
                                closestBuddy = iteratableBuddy;
                        }
                    }


                    if (usingTestingMob)
                    {
                        //Debug.Log($"For '{testingMobGOName}', closest buddy is: '{closestBuddy.GetTransform().gameObject.name}'.");
                    }

                    // TODO Если мы влезли в радиус существа (CreatureMeasures.radius), тогда инициируем отход.
                }
            }
        }

        private void TryRotateToTarget()
        {
            bool canRotate = _npcAI.CanRotate &&
                             (_npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility == false || (_npcAI._isStopMoveWhenCastingNonInstantAbility && _npcAI.INpcBaseCreature.AbilitiesController.IsCastingAbility) == false);

            if (canRotate)
            {
                Vector3 directionToTarget = _npcAI.NPCTargetHandler.SelectedTarget.IBaseCreature.GetRootObjectTransform().position - _npcAI.INpcBaseCreature.GetRootObjectTransform().position;
                RotateToDirection(directionToTarget);
                // Vector3 directionToDestination = _aiPath.destination - _npcAI.INpcBaseCreature.GetRootObjectTransform().position;
                // RotateToDirection(directionToDestination);
            }
        }

        private void RotateToDirection(Vector3 directionToTarget)
        {
            if (directionToTarget == Vector3.zero)
                return;

            float dotProduct = Vector3.Dot(directionToTarget.normalized, _npcAI.INpcBaseCreature.GetRootObjectTransform().forward);
            if (dotProduct < ROTATION_DOT_MAX_VAL)
            {
                Vector3 directionToTargetWithoutY = new Vector3(directionToTarget.x, 0, directionToTarget.z);
                if (directionToTargetWithoutY != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToTargetWithoutY);
                    Quaternion rotateTowards;

                    if (_npcAI.AIPath == null)
                        rotateTowards = lookRotation;
                    else
                        rotateTowards = Quaternion.RotateTowards(_npcAI.INpcBaseCreature.GetRootObjectTransform().rotation, lookRotation, _npcAI.AIPath.rotationSpeed * Time.deltaTime);

                    _npcAI.INpcBaseCreature.GetRootObjectTransform().rotation = rotateTowards;
                }
            }
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            OnEnterStateCaching(); // caching some parameters

            if (_npcAI.AIPath != null)
            {
                _npcAI.AIPath.onSearchPath = ONSearchPath;
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();

            if (_npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility)
            {
                _abilitiesController.InterruptCast();
                ResetCastingInProgressState();
            }

            _npcBaseCreature.BuffsController.ResetRuntimeBuffs();

            //_npcBaseCreature.CombatInfoHandler.DisengageCombat(_npcAI.NPCTargetHandler.SelectedTarget.IBaseCreature);

            if (_npcAI.AIPath != null)
            {
                _npcAI.AIPath.destination = Vector3.positiveInfinity;
                _npcAI.AIPath.onSearchPath = null;
                
                _npcAI.AIPath.SetSlowdownDistance(_npcAI.DefaultSlowdownDistance);
                _npcAI.AIPath.SetEndReachedDistance(_npcAI.DefaultEndReachedDistance);
            }
            
            // !float.IsPositiveInfinity(destination.x)

            _npcAI.NPCTargetHandler.RemoveTarget();
        }

        private void OnEnterStateCaching()
        {
            CachedOnEnterPosition = _npcBaseCreature.GetRootObjectTransform().position; // save position
            CachedOnEnterRotation = _npcBaseCreature.GetRootObjectTransform().rotation; // save rotation
        }
    }
}