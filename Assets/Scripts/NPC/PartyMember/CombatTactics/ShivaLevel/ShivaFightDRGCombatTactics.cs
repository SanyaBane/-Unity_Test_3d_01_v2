using Assets.Scripts.Levels.ShivaLevel;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics.ShivaLevel
{
    public class ShivaFightDRGCombatTactics : DRGCombatTactics
    {
        private readonly ShivaLevelManager _shivaLevelManager;
        
        public ShivaFightDRGCombatTactics(NpcAI npcAI, ShivaLevelManager shivaLevelManager) : base(npcAI)
        {
            _shivaLevelManager = shivaLevelManager;
            
            _shivaLevelManager.BattleEnded += ShivaLevelManagerOnBattleEnded;
            _shivaLevelManager.BossPooled += ShivaLevelManagerOnBossPooled;
        }

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
            var offsetFromTargetCenter = _shivaLevelManager.ShivaBossCreature.CreatureMeasures.Radius
                                         + PreferableAttackDistance;
            
            AttackTargetAtPosition(_shivaLevelManager.ShivaBossCreature.GetGroundedPosition(), offsetFromTargetCenter, rotationToTarget);
            
            ExecuteCombatRotation();
        }
    }
}