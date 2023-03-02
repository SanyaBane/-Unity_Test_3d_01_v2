using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public abstract class BaseNpcAiState : BaseState
    {
        public NpcAI NpcAI { get; }

        public enum AiStateTypeEnum
        {
            Custom,
            Battle,
            Peaceful
        }
        
        public abstract AiStateTypeEnum EAiStateType { get; }

        public override bool IsAllowedToMove()
        {
            if (NpcAI._isStopMoveWhenCastingNonInstantAbility && NpcAI.INpcBaseCreature.AbilitiesController.IsCastingAbility)
            {
                return false;
            }

            if (!NpcAI.CanMove || !NpcAI.CanMoveToTarget)
            {
                return false;
            }

            return true;
        }

        public BaseNpcAiState(NpcAI npcAI) : base(npcAI)
        {
            NpcAI = npcAI;
        }
        
        public override void TickState()
        {
            base.TickState();

            switch (EAiStateType)
            {
                case AiStateTypeEnum.Custom:
                    break;
                
                case AiStateTypeEnum.Battle:
                    NpcAI.CombatTacticsAI.ProcessCombatTactics();
                    break;
                
                case AiStateTypeEnum.Peaceful:
                    NpcAI.PeaceTacticsAI.ProcessPeaceTactics();
                    break;
            }
        }
    }
}