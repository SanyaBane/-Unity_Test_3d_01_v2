using Assets.Scripts.Interfaces;
using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class MoveToAttackTargetState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;
        public IBaseCreature Target;
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Battle;
        
        public MoveToAttackTargetState(NpcAI npcAI, Animator animator) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
        }

        public override void TickState()
        {
            base.TickState();
            
            _npcAI.CombatTacticsAI.ResetCombatTactics();
            
            _npcAI.AIPath.destination = Target.GetGroundedPosition();
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            if (Target == null)
            {
                Debug.LogError("Target == null");
                return;
            }
            
            _npcAI.NPCTargetHandler.SelectTarget(Target.ITargetable);

            //_animator.SetBool(ConstantsAnimator.AUTO_ATTACK_BOOL_WEAPON_SHEATHED, false);
            
            // _npcAI.AIDestinationSetter.target = Target.GetRootObjectTransform();
            _npcAI.AIPath.destination = Target.GetGroundedPosition();
        }

        public override void OnExitState()
        {
            base.OnExitState();

            // _npcAI.AIDestinationSetter.target = null;
            _npcAI.AIPath.destination = Vector3.positiveInfinity;

            _npcAI.NPCTargetHandler.RemoveTarget();
            
            Target = null;
        }
    }
}