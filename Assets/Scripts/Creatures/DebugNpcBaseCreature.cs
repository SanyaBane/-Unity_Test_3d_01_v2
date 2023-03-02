using System;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class DebugNpcBaseCreature : MonoBehaviour
    {
        public NpcBaseCreature NpcBaseCreature;
        
        public DebugInfoAboveGameObject InfoAiState;
        public DebugInfoAboveGameObject TargetInfo;

        private void Update()
        {
            var combatTarget = NpcBaseCreature.NpcAI.CombatTacticsAI.CurrentTarget;
            if (combatTarget != null)
            {
                var groundedDistance = Vector3.Distance(NpcBaseCreature.GetGroundedPosition(), combatTarget.IBaseCreature.GetGroundedPosition());
                TargetInfo.FloatingText.Text = $"Distance to target: {groundedDistance}";
            }
        }
    }
}