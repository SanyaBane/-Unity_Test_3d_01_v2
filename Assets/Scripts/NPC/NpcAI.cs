using Assets.Scripts.AutoAttack;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.TargetHandling;
using Pathfinding;
using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities;
using Assets.Scripts.AreaOfEffects;
using Assets.Scripts.Buffs;
using Assets.Scripts.NPC.Tactics;
using Assets.Scripts.StateMachineScripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(NpcTargetHandler))]
    public class NpcAI : MonoBehaviour
    {
        protected StateMachine _stateMachine;
        
        // public enum BehaviorEnum
        // {
        //     Defensive,
        //     Aggressive,
        //     Passive
        // }
        //
        // public BehaviorEnum EBehavior = BehaviorEnum.Defensive;

        public Seeker Seeker { get; private set; }
        public AIPath AIPath { get; private set; }
        public AIDestinationSetter AIDestinationSetter { get; private set; }

        private const float ANIMATOR_MOVEMENT_VELOCITY_THRESHOLD = 0.01f;

        public INpcBaseCreature INpcBaseCreature { get; private set; }

        protected Animator _animator => INpcBaseCreature.Animator;
        protected AbilitiesController _abilitiesController => INpcBaseCreature.AbilitiesController;
        protected AutoAttackController _autoAttack => INpcBaseCreature.AutoAttackController;
        public NPCHealth _npcHealth => INpcBaseCreature.Health as NPCHealth;
        protected BuffsController _buffsController => INpcBaseCreature.BuffsController;
        public NpcTargetHandler NPCTargetHandler => INpcBaseCreature.NPCTargetHandler;

        public float EscapeFromAoeSlowdownDistance = 0.8f;
        public float EscapeFromAoeEndReachedDistance = 0.2f;
        public float EscapeFromAoeSafeAreaOffset = 0.2f;

        public float DefaultSlowdownDistance = 3.0f;
        public float DefaultEndReachedDistance = 1.5f;
        
        public float MaxSpeed = 6;

        [Header("General")]

        #region CanMove

        [SerializeField] private bool _canMove = true;

        public bool CanMove
        {
            get => _canMove;
            private set
            {
                _canMove = value;
                UpdateCanMove();
                CanMoveChanged?.Invoke(_canMove);
            }
        }

        #endregion

        #region CanMoveToTarget

        [SerializeField] private bool _canMoveToTarget = true;
        public bool CanMoveToTarget
        {
            get => _canMoveToTarget;
            private set
            {
                _canMoveToTarget = value;
                UpdateCanMove();
                CanMoveToTargetChanged?.Invoke(_canMoveToTarget);
            }
        }

        #endregion

        [SerializeField] private bool _canRotate = true;
        public bool CanRotate
        {
            get => _canRotate;
            set
            {
                _canRotate = value;
                CanRotateChanged?.Invoke(_canRotate);
            }
        }
        
        public EJob Job = EJob.None;

        public ERole Role = ERole.None;

        public Action<bool> CanMoveChanged;
        public Action<bool> CanMoveToTargetChanged;
        public Action<bool> CanRotateChanged;

        private void UpdateCanMove()
        {
            // if (!Application.isEditor)
            // {
            if (AIPath != null)
            {
                AIPath.enabled = _stateMachine._currentState.IsAllowedToMove();
                // AIPath.canMove = _stateMachine._currentState.IsAllowedToMove();
            }
            // }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CanMove = _canMove;
            CanMoveToTarget = _canMoveToTarget;
            CanRotate = _canRotate;
        }
#endif

        [SerializeField] private float _engageDistance = 6.0f;
        [SerializeField] private bool _isDisengageOnDistance = true;
        [SerializeField] private float _disengageDistance = 9.0f;

        public float GetCompleteEngageDistance()
        {
            return _engageDistance; // + INpcBaseCreature.CreatureMeasures.Radius;
        }

        public bool IsDisengageOnDistance => _isDisengageOnDistance;

        public float GetCompleteDisengageDistance()
        {
            return _disengageDistance; // + INpcBaseCreature.CreatureMeasures.Radius;
        }

        protected PatrolStateController PatrolStateController { get; private set; }
        
        protected DyingState dyingState;
        protected IdleState idleState;
        protected PatrolState patrolState;
        protected PatrolShortBreakState patrolShortBreakState;
        protected ReturnToPositionBeforeCombatState returnToPositionBeforeCombatState;
        protected BaseCombatState baseCombatState;
        protected MoveToAttackTargetState moveToAttackTargetState;
        protected FollowTargetState followTargetState;
        protected GoToTargetState goToTargetState;
        public BaseCombatState GetCombatState() => baseCombatState;

        public bool CanSwitchToCombatState { get; set; } = true;

        public BaseAIPeaceTactics PeaceTacticsAI { get; protected set; }
        public BaseAICombatTactics CombatTacticsAI { get; protected set; }

        protected virtual void SetPeaceTactics()
        {
            PeaceTacticsAI = GameManager.Instance.AiManager.BaseLevelManager.GetPeaceTactics(this);
        }

        protected virtual void SetCombatTactics()
        {
            CombatTacticsAI = GameManager.Instance.AiManager.BaseLevelManager.GetCombatTactics(this);
        }

        #region Ability-related activities

        public bool _isStopMoveWhenCastingNonInstantAbility => true;

        public bool CanTryCast(Ability ability)
        {
            if (CombatTacticsAI.IsPrioritizeMovingOverDamageDealing && !ability.IsCastInstant(INpcBaseCreature))
                return false;
            
            var ret = INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility == false && 
                           ability.AbilityCooldown.TimeUntilCooldownFinish == 0;
            return ret;
        }

        /// <param name="abilityParameters">Can be null.</param>
        public void TryCastAbility(Ability ability)
        {
            _abilitiesController.TryStartCast(ability);
        }

        public virtual float GetLevelCircleAreaRadius()
        {
            return 0;
        }

        #endregion

        public bool IsShouldEvadeAOE = false;
        public bool IsEvadingAOE = false;

        [Header("Debugging")]
        public bool DrawEngageDistance = false;

        public bool DrawDisengageDistance = false;

        public bool NavMeshTargetSphereIsDraw = false;
        public float NavMeshTargetSphereRadius = 0.25f;

#pragma warning disable IDE0052 // Remove unread private members
        [SerializeField] private string CurrentStateName;
#pragma warning restore IDE0052 // Remove unread private members

        private void Awake()
        {
            INpcBaseCreature = GetComponent<INpcBaseCreature>();

            PatrolStateController = GetComponent<PatrolStateController>();
        }

        protected virtual void Start()
        {
            Seeker = CreatureHelper.GetSeekerFromIBaseCreature(INpcBaseCreature);
            AIPath = CreatureHelper.GetAIPathFromIBaseCreature(INpcBaseCreature);
            AIDestinationSetter = CreatureHelper.GetAIDestinationSetterFromIBaseCreature(INpcBaseCreature);

            SetupStateMachine();

            SetPeaceTactics();
            SetCombatTactics();
        }

        [HideInInspector] public List<Vector3> ListSafePointsOutsideAOECircles = new List<Vector3>();
        [HideInInspector] public List<Vector3> ListMultipleAOECirclesIntersections = new List<Vector3>();
        [HideInInspector] public List<Vector3> ListAOECirclePieLinesIntersectWithAnotherCircleRadius = new List<Vector3>();
        [HideInInspector] public List<Vector3> ListAOECirclePieLinesIntersectWithAnotherCirclePieLines = new List<Vector3>();
        [HideInInspector] public List<Vector3> ListAOECircleCenterWithOffset = new List<Vector3>();

        public readonly List<AOECircleDebugIntersectionLineWithPieLinesNotPieArea> ListDebugIntersectionLineWithPieLinesNotPieArea = new List<AOECircleDebugIntersectionLineWithPieLinesNotPieArea>();
        public readonly List<AOECircleDebugIntersectionLineWithPieLinesNotPieArea> ListDebugIntersectionAOEPieLineWithSecondAOEPieLine = new List<AOECircleDebugIntersectionLineWithPieLinesNotPieArea>();

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (NavMeshTargetSphereIsDraw)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(AIPath.destination, NavMeshTargetSphereRadius);
            }

            // ==================================================================================

            Gizmos.color = Color.blue;
            foreach (var point in ListSafePointsOutsideAOECircles)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }

            Gizmos.color = Color.yellow;
            foreach (var point in ListMultipleAOECirclesIntersections)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }

            foreach (var element in ListDebugIntersectionLineWithPieLinesNotPieArea)
            {
                element.DisplayGizmos();
            }

            foreach (var element in ListDebugIntersectionAOEPieLineWithSecondAOEPieLine)
            {
                element.DisplayGizmos();
            }

            Gizmos.color = Color.magenta;
            foreach (var point in ListAOECirclePieLinesIntersectWithAnotherCircleRadius)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }

            Gizmos.color = Color.green;
            foreach (var point in ListAOECirclePieLinesIntersectWithAnotherCirclePieLines)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }

            Gizmos.color = Color.white;
            foreach (var point in ListAOECircleCenterWithOffset)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }

            // ==================================================================================

            if (DrawEngageDistance)
            {
                Handles.color = new Color(1, 0.92f, 0.01f);
                Handles.DrawWireArc(INpcBaseCreature.GetRootObjectTransform().position, Vector3.up, Vector3.forward, 360, GetCompleteEngageDistance());
            }

            if (DrawDisengageDistance)
            {
                Handles.color = new Color(1, 0.72f, 0.01f);
                Handles.DrawWireArc(INpcBaseCreature.GetRootObjectTransform().position, Vector3.up, Vector3.forward, 360, GetCompleteDisengageDistance());
            }
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            _stateMachine.OnStateChanged += OnStateChanged;

            dyingState = new DyingState(this, _animator, AIPath);
            idleState = new IdleState(this, _animator);
            returnToPositionBeforeCombatState = new ReturnToPositionBeforeCombatState(this, _animator, _npcHealth, _buffsController, AIPath);
            returnToPositionBeforeCombatState.EnteredReturnToPositionBeforeCombatState += OnEnteredReturnToPositionBeforeCombatState;

            if (PatrolStateController != null)
            {
                patrolState = new PatrolState(this, AIPath, AIDestinationSetter, _animator, PatrolStateController);
                patrolShortBreakState = new PatrolShortBreakState(this, _animator, PatrolStateController);

                _stateMachine.AddTransition(idleState, patrolState, () => PatrolStateController.IsInPatrolingMode && PatrolStateController.CanPatrol);
                _stateMachine.AddTransition(patrolState, patrolShortBreakState, PatrolStateController.PatrolingReachedDestination());
                _stateMachine.AddTransition(patrolShortBreakState, patrolState, patrolShortBreakState.PatrolingShortBreakFinished());

                PatrolStateController.IsInPatrolingMode = true; // startup value
            }

            _stateMachine.AddAnyTransition(dyingState, Dying());

            _stateMachine.SetState(idleState); // default state

            moveToAttackTargetState = new MoveToAttackTargetState(this, _animator);
            followTargetState = new FollowTargetState(this, _animator);
            goToTargetState = new GoToTargetState(this, _animator);

            // _stateMachine.AddAnyTransition(moveToAttackTarget, () => false);
            // _stateMachine.AddTransition(moveToAttackTarget, idleState, () => AIPath.reachedDestination);


            // _stateMachine.AddAnyTransition(followTarget, () => false);
            // _stateMachine.AddAnyTransition(goToTarget, () => false);

            SetupCombatState();

            _stateMachine.AddTransition(goToTargetState, idleState, () => AIPath.reachedDestination);

            Func<bool> Dying() => () => { return !INpcBaseCreature.Health.IsAlive; };
        }

        private void OnStateChanged(IState currentState, IState previousState)
        {
            INpcBaseCreature.DebugNpcBaseCreature.InfoAiState.FloatingText.Text = currentState.GetType().Name;
        }

        protected virtual void OnEnteredReturnToPositionBeforeCombatState()
        {
        }

        public void MoveToAttackTarget(IBaseCreature iBaseCreature)
        {
            moveToAttackTargetState.Target = iBaseCreature;
            _stateMachine.SetState(moveToAttackTargetState);
        }

        public void FollowTarget(IBaseCreature iBaseCreature)
        {
            followTargetState.Target = iBaseCreature;
            _stateMachine.SetState(followTargetState);
        }

        public void GoToTarget(IBaseCreature iBaseCreature)
        {
            goToTargetState.Target = iBaseCreature;
            _stateMachine.SetState(goToTargetState);
        }

        #region Attack

        private BaseCombatState CreateCombatState()
        {
            var ret = new BaseCombatState(this, INpcBaseCreature, _animator, _abilitiesController, _autoAttack);
            return ret;
        }

        protected virtual void SetupCombatState()
        {
            baseCombatState = CreateCombatState();

            _stateMachine.AddAnyTransition(baseCombatState, () => CanTransitToCombatState);
            _stateMachine.AddTransition(baseCombatState, returnToPositionBeforeCombatState, LostTargetToAttack());
            _stateMachine.AddTransition(returnToPositionBeforeCombatState, idleState, returnToPositionBeforeCombatState.CanSwitchToIdle());

            // _stateMachine.AddTransition(moveToAttackTarget, baseCombatState, () => AIPath.reachedDestination);
        }

        protected Func<bool> FoundTargetToAttack() => () => { return INpcBaseCreature.CombatInfoHandler.IsInCombat && CanLookForEnemies && INpcBaseCreature.Health.IsAlive; };
        protected Func<bool> LostTargetToAttack() => () => { return INpcBaseCreature.CombatInfoHandler.IsInCombat == false && INpcBaseCreature.Health.IsAlive; };
        protected Func<bool> ReachedDestination() => () => { return AIPath.reachedDestination; };
        
        public bool CanTransitToCombatState => CanSwitchToCombatState && FoundTargetToAttack().Invoke();

        #endregion

        public bool CanLookForEnemies => _npcHealth.IsAlive &&
                                          _stateMachine._currentState != returnToPositionBeforeCombatState;

        private void Update()
        {
            _stateMachine.Tick();

            CurrentStateName = _stateMachine._currentState.GetType().Name;
            if (CurrentStateName == nameof(ReturnToPositionBeforeCombatState))
            {
            }

            CombatTacticsAI.AttemptToFindEnemy();
            // if (!INpcBaseCreature.CombatInfoHandler.GetCombatInfos().Any())
            // {
            //     if (CanLookForEnemies)
            //     {
            //         var foundedNearestEnemy = CheckAroundForEnemy();
            //         if (foundedNearestEnemy != null)
            //         {
            //             bool alreadyEngagedWithCreature = INpcBaseCreature.CombatInfoHandler.IsAlreadyEngagedWithCreature(foundedNearestEnemy);
            //             if (!alreadyEngagedWithCreature)
            //             {
            //                 INpcBaseCreature.CombatInfoHandler.EngageCombat(foundedNearestEnemy);
            //             }
            //         }
            //     }
            // }

            // string testingMobGOName = "PartyMember_Ardbert";
            // bool usingTestingMob = INpcBaseCreature.GetRootObjectTransform().gameObject.name == testingMobGOName;
            // if (usingTestingMob)
            // {
            // }

            UpdateCanMove();

            if (AIPath != null &&
                AIPath.enabled &&
                // AIPath.canMove &&
                AIPath.reachedDestination == false &&
                AIPath.reachedEndOfPath == false &&
                AIPath.velocity.magnitude > ANIMATOR_MOVEMENT_VELOCITY_THRESHOLD)
                // (_aiPath.velocity.x > velocityThreshold || _aiPath.velocity.z > velocityThreshold))
            {
                _animator.SetBool(ConstantsAnimator.PLAYER_CONTROLLER_BOOL_IS_RUNNING, true);
                _animator.SetFloat(ConstantsAnimator.NPC_AI_FLOAT_VELOCITY_Y, 1);
            }
            else
            {
                _animator.SetBool(ConstantsAnimator.PLAYER_CONTROLLER_BOOL_IS_RUNNING, false);
                _animator.SetFloat(ConstantsAnimator.NPC_AI_FLOAT_VELOCITY_Y, 0);
            }
        }

        
    }
}