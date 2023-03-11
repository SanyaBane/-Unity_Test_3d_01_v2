using Assets.Scripts.Levels;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics.ShivaLevel
{
    public class ShivaFightBLMCombatTactics : BLMCombatTactics
    {
        private readonly ShivaLevelManager _shivaLevelManager;
        
        public ShivaFightBLMCombatTactics(NpcAI npcAI, ShivaLevelManager shivaLevelManager) : base(npcAI)
        {
            _shivaLevelManager = shivaLevelManager;
            
            _shivaLevelManager.BattleEnded += ShivaLevelManagerOnBattleEnded;
            _shivaLevelManager.BossPooled += ShivaLevelManagerOnBossPooled;
        }
        
        public override float PreferableAttackDistance => 6.0f;

        private void ShivaLevelManagerOnBossPooled()
        {
        }

        private void ShivaLevelManagerOnBattleEnded()
        {
            ResetCombatTactics();
        }
        
        public override void ResetCombatTactics()
        {
            IsCombatTacticsHandleMoving = false;
        }
        
        public override void ProcessCombatTactics()
        {
            IsPrioritizeMovingOverDamageDealing = false;
            
            IsCombatTacticsHandleMoving = true;
            
            CurrentTarget = _shivaLevelManager.ShivaBossCreature.ITargetable;
            
            // var targetBaseCreature = NpcAI.INpcBaseCreature.ICanSelectTarget.SelectedTarget.IBaseCreature;
            // var rotationToTarget = targetBaseCreature.GetRootObjectTransform().rotation.eulerAngles;
            var rotationToTarget = Vector3.back;
            AttackTargetAtPosition(_shivaLevelManager.ShivaBossCreature.GetGroundedPosition(), PreferableAttackDistance, rotationToTarget);
            
            ExecuteCombatRotation();
        }
    }
}