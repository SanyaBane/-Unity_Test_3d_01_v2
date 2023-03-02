using Assets.Scripts.StateMachineScripts;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class DyingState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;
        private readonly AIPath _aiPath;

        public override bool IsAllowedToMove()
        {
            return false;
        }
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Custom;
        
        public DyingState(NpcAI npcAI, Animator animator, AIPath aiPath) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
            _aiPath = aiPath;
        }

        public override void TickState()
        {
            base.TickState();
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            //
            // if (_aiPath != null)
            //     _aiPath.canMove = false;
        }

        public override void OnExitState()
        {
            base.OnExitState();
        }
    }
}