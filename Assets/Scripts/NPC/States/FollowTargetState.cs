using Assets.Scripts.Interfaces;
using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class FollowTargetState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;
        public IBaseCreature Target;

        // private bool _isAllowedToMove;
        // public override bool IsAllowedToMove()
        // { 
        //     return base.IsAllowedToMove() && _isAllowedToMove;
        // }

        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Peaceful;
        
        public FollowTargetState(NpcAI npcAI, Animator animator) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
        }

        public override void TickState()
        {
            base.TickState();
            
            _npcAI.AIPath.destination = Target.GetGroundedPosition();
            
            // var distanceToTargetSquared = VectorHelper.DistanceSquared(_npcAI.INpcBaseCreature.GetRootObjectTransform().position, Target.GetRootObjectTransform().position);
            // if (distanceToTargetSquared <= _npcAI.AIPath.endReachedDistance * _npcAI.AIPath.endReachedDistance)
            // {
            //     _isAllowedToMove = false;
            // }
            // else
            // {
            //     _isAllowedToMove = true;
            // }
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            if (Target == null)
            {
                Debug.LogError("Target == null");
                return;
            }

            if (_npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility)
            {
                _npcAI.INpcBaseCreature.AbilitiesController.InterruptCast();
            }
            
            // _npcAI.AIPath.canMove = true;

            _npcAI.CanSwitchToCombatState = false;
            
            // _npcAI.AIPath.maxSpeed = _npcAI.MaxSpeed;

            // _npcAI.AIDestinationSetter.target = Target.GetRootObjectTransform();
            _npcAI.AIPath.destination = Target.GetGroundedPosition();
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _npcAI.AIDestinationSetter.target = null;
            // _npcAI.AIPath.destination = Vector3.positiveInfinity;
            
            _npcAI.CanSwitchToCombatState = true;
            
            // _npcAI.AIPath.maxSpeed = 0;
            // _npcAI.AIPath.enabled = true;
            // _npcAI.AIPath.canMove = true;
            
            // _npcAI.AIPath.velocity = Vector3.zero;

            Target = null;
        }
    }
}