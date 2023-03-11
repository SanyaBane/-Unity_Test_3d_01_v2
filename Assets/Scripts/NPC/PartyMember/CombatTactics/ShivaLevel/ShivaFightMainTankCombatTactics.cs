using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Levels.ShivaLevel;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics.ShivaLevel
{
    public class ShivaFightMainTankCombatTactics : MainTankCombatTactics
    {
        private readonly ShivaLevelManager _shivaLevelManager;

        private int _usedTankRangeAbilityCounter;

        public ShivaFightMainTankCombatTactics(NpcAI npcAI, ShivaLevelManager shivaLevelManager) : base(npcAI)
        {
            _shivaLevelManager = shivaLevelManager;
            
            _shivaLevelManager.BattleEnded += ShivaLevelManagerOnBattleEnded;
        }

        private void ShivaLevelManagerOnBattleEnded()
        {
            ResetCombatTactics();
        }
        
        protected override void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            base.AbilitiesControllerOnCastFinished(abilitiesController, ability);
            
            if (ability == _tankRangeAbility)
            {
                _usedTankRangeAbilityCounter++;
            }
        }

        public override void ResetCombatTactics()
        {
            IsCombatTacticsHandleMoving = false;
            _usedTankRangeAbilityCounter = 0;
        }

        public override void ProcessCombatTactics()
        {
            IsPrioritizeMovingOverDamageDealing = false;
            
            if (_usedTankRangeAbilityCounter == 0 && _shivaLevelManager.TimeAfterBossPull < 3.0f)
            {
                NpcAI.NPCTargetHandler.SelectTarget(_shivaLevelManager.ShivaBossCreature.ITargetable);
                NpcAI.TryCastAbility(_tankRangeAbility);
            }

            var shivaCombatInfoMaxThreat = _shivaLevelManager.ShivaBossCreature.NpcAI.GetCombatState().AggroController.GetPrimaryTarget();
            if (shivaCombatInfoMaxThreat == null)
                return;

            if (ReferenceEquals(shivaCombatInfoMaxThreat, NpcAI.INpcBaseCreature))
            {
                IsCombatTacticsHandleMoving = true;
                TankTargetAtPosition(_shivaLevelManager.ShivaBossCreature, _shivaLevelManager.PositionWhereToTankBoss, Vector3.forward);
            }
            else
            {
                IsCombatTacticsHandleMoving = false;
                // _partyMemberAI.IsMoveToTarget = true;
            }
            
            ExecuteCombatRotation();
        }
    }
}