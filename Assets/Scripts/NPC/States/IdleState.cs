using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class IdleState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;

        // private float _cachedMaxSpeed = 0;
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Peaceful;

        public override bool IsAllowedToMove()
        {
            return false;
        }

        public IdleState(NpcAI npcAI, Animator animator) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            if (_npcAI.AIPath != null)
            {
                _npcAI.AIPath.maxSpeed = _npcAI.MaxSpeed;
            }

            // _cachedMaxSpeed = _npcAI.AIPath.maxSpeed;
            // _npcAI.AIPath.maxSpeed = 0;

            //_animator.SetInteger("Idle", 1);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            // _npcAI.AIPath.maxSpeed = _cachedMaxSpeed;

            //_animator.SetInteger("Idle", 0);
        }
    }
}