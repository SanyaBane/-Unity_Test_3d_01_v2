using System;
using System.Linq;
using Assets.Scripts.Abilities;
using Assets.Scripts.Extensions;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Interfaces;
using Assets.Scripts.NPC.Tactics;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics
{
    public abstract class DamageDealerCombatTactics : BaseAICombatTactics
    {
        public DamageDealerCombatTactics(NpcAI _npcAI) : base(_npcAI)
        {
        }


        public override void ProcessCombatTactics()
        {
            ExecuteCombatRotation();
        }

        protected abstract void ExecuteCombatRotation();

        protected void AttackTargetAtPosition(Vector3 targetPosition, float distanceFromTargetCenterInUnits, Vector3 offsetPositionRotation)
        {
            bool isChangedAIPathDistances = false;
            
            Vector3 offSetPositionConsideringRotation = offsetPositionRotation * distanceFromTargetCenterInUnits;

            // var destination = target.GetGroundedPosition() + offSetPositionConsideringRotation;
            var destination = PartyMemberHelper.GetBattlePosition(NpcAI.Role, targetPosition + offSetPositionConsideringRotation, offsetPositionRotation);
            
            const float tolerance = 0.1f;
            var distanceSquaredCreatureAndNeededPosition = VectorHelper.DistanceForComparison(NpcAI.INpcBaseCreature.GetGroundedPosition(), destination);
            var onPosition = distanceSquaredCreatureAndNeededPosition < VectorHelper.GetDistanceComparisonValue(tolerance);
            if (!onPosition)
            {
                NpcAI.AIPath.destination = destination;
                NpcAI.AIPath.SetEndReachedDistance(0.05f);;
                NpcAI.AIPath.SetSlowdownDistance(0.3f);
                isChangedAIPathDistances = true;
                IsPrioritizeMovingOverDamageDealing = true;
            }
            // else
            // {
            //     //NpcAI.AIPath.SetPath(null);
            //     // NpcAI.AIPath.path
            // }
            
            if (!isChangedAIPathDistances)
            {
                if (NpcAI.AIPath != null)
                {
                    NpcAI.AIPath.SetSlowdownDistance(NpcAI.DefaultSlowdownDistance);
                    NpcAI.AIPath.SetEndReachedDistance(NpcAI.DefaultEndReachedDistance);
                }
            }
        }
    }
}