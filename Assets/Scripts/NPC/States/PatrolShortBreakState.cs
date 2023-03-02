using System;
using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class PatrolShortBreakState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;
        private readonly PatrolStateController _patrolStateController;

        public float CurrentBreakValue { get; private set; }

        public override bool IsAllowedToMove()
        {
            return false;
        }
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Peaceful;

        public PatrolShortBreakState(NpcAI npcAI, Animator animator, PatrolStateController patrolStateController) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
            _patrolStateController = patrolStateController;
        }

        public override void TickState()
        {
            base.TickState();

            CurrentBreakValue += Time.deltaTime;
        }

        public Func<bool> PatrolingShortBreakFinished() => () =>
        {
            var ret = CurrentBreakValue >= _patrolStateController.BreakWhilePatroling;
            return ret;
        };

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            CurrentBreakValue = 0;

            _patrolStateController.PatrolCurrentDestination = null; //set to null, so when next Patrol will be started, it will pick new destination

            //_animator.SetInteger("Idle", 1);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            //_animator.SetInteger("Idle", 0);
        }
    }
}