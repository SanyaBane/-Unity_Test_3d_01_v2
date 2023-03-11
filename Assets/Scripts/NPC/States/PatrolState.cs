using Assets.Scripts.StateMachineScripts;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class PatrolState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly AIPath _aiPath;
        private readonly AIDestinationSetter _aiDestinationSetter;
        private readonly Animator _animator;
        private readonly PatrolStateController _patrolStateController;
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Peaceful;

        public PatrolState(NpcAI npcAI, AIPath aiPath, AIDestinationSetter aiDestinationSetter, Animator animator, PatrolStateController patrolStateController) : base(npcAI)
        {
            _npcAI = npcAI;
            _aiPath = aiPath;
            _aiDestinationSetter = aiDestinationSetter;
            _animator = animator;
            _patrolStateController = patrolStateController;
        }

        public override void TickState()
        {
            base.TickState();

            if (_patrolStateController.PatrolCurrentDestination == null)
            {
                _patrolStateController.PatrolCurrentDestination = GetNextDestination();

                _aiPath.destination = _patrolStateController.PatrolCurrentDestination.Value;
            }
        }

        private Vector3 GetNextDestination()
        {
            if (_patrolStateController.Destinations.Length == 0)
                return _npcAI.INpcBaseCreature.GetRootObjectTransform().position;

            _patrolStateController.DestinationCurrentIndex++;
            if (_patrolStateController.DestinationCurrentIndex >= _patrolStateController.Destinations.Length)
                _patrolStateController.DestinationCurrentIndex = 0;

            return _patrolStateController.Destinations[_patrolStateController.DestinationCurrentIndex].position;
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            // _aiPath.canMove = true;

            // продолжить патруль если он был прерван
            if (_patrolStateController.PatrolCurrentDestination != null)
            {
                _aiPath.destination = _patrolStateController.PatrolCurrentDestination.Value;
            }

            //_animator.SetInteger("Run", 1);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            //_navMeshAgent.enabled = false;
            //_animator.SetInteger("Run", 0);
        }
    }
}