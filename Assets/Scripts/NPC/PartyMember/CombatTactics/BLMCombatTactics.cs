using System.Linq;
using Assets.Scripts.Abilities;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics
{
    public class BLMCombatTactics : DamageDealerCombatTactics
    {
        private const int MANA_AMOUNT_NEEDED_TO_START_COMBAT_ROTATION = 9200;
        
        private AbilityTargetProjectile _fire1;
        private AbilityTarget _fire3;
        private AbilityTarget _blizzard1;
        private AbilityTarget _blizzard3;
        private AbilityTargetProjectile _thunder3;

        private AbilityTarget _transpose;

        public BLMCombatTactics(NpcAI npcAI) : base(npcAI)
        {
            _fire1 = (AbilityTargetProjectile) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_FIRE1);
            _fire3 = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_FIRE3);
            _blizzard1 = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_BLIZZARD1);
            _blizzard3 = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_BLIZZARD3);
            _thunder3 = (AbilityTargetProjectile) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_THUNDER3);
            _transpose = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_BLM.ABILITY_BLM_TRANSPOSE);
        }
        
        public override float PreferableAttackDistance => Constants.BLM_PREFERRED_ATTACK_DISTANCE;

        public override void ProcessCombatTactics()
        {
            ExecuteCombatRotation();
        }
        
        protected override void ExecuteCombatRotation()
        {
            if (NpcAI.INpcBaseCreature.ICanSelectTarget.SelectedTarget == null)
                return;

            if (NpcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility)
                return;
            
            if (IsPrioritizeMovingOverDamageDealing)
                return;

            var debuffThunder3 = NpcAI.INpcBaseCreature.ICanSelectTarget.SelectedTarget.IBaseCreature.BuffsController.GetBuffById(ConstantsAbilities_BLM.DEBUFF_BLM_THUNDER3);

            var buffAstralFire = NpcAI.INpcBaseCreature.BuffsController.GetBuffById(ConstantsAbilities_BLM.BUFF_BLM_FIRE_STACKS);
            var buffUmbralIce = NpcAI.INpcBaseCreature.BuffsController.GetBuffById(ConstantsAbilities_BLM.BUFF_BLM_ICE_STACKS);

            var thunder3ProjectileIsAlreadyOnTheWayToTarget = NpcAI.INpcBaseCreature.ICanSelectTarget.SelectedTarget.ProjectilesOnTheWay
                .Any(x => x.AbilityTargetProjectile.AbilitySO.Id == ConstantsAbilities_BLM.ABILITY_BLM_THUNDER3 && x.Source == NpcAI.INpcBaseCreature);
            
            bool shouldCastThunder3IfPossible = (debuffThunder3 == null ||
                                                 debuffThunder3.BuffDuration.RemainingDuration < debuffThunder3.BuffDuration.BaseBuffDurationSO.InitialDuration / 2) &&
                                                !thunder3ProjectileIsAlreadyOnTheWayToTarget;

            if (NpcAI.INpcBaseCreature.ManaController.CurrentMP >= MANA_AMOUNT_NEEDED_TO_START_COMBAT_ROTATION)
            {
                if (buffUmbralIce == null && buffAstralFire == null)
                {
                    NpcAI.TryCastAbility(_blizzard3);
                    return;
                }

                // Buffed by Umbral Ice
                if (buffUmbralIce != null)
                {
                    if (buffUmbralIce.StacksCount == 3)
                    {
                        if (shouldCastThunder3IfPossible)
                        {
                            if (NpcAI.CanTryCast(_thunder3))
                            {
                                NpcAI.TryCastAbility(_thunder3);
                                return;
                            }
                        }
                        else
                        {
                            NpcAI.TryCastAbility(_fire3);
                            return;
                        }
                    }
                    else
                    {
                        NpcAI.TryCastAbility(_blizzard3);
                        return;
                    }
                }

                // Buffed by Astral Fire
                if (buffAstralFire != null)
                {
                    if (buffAstralFire.StacksCount == 3)
                    {
                        NpcAI.TryCastAbility(_fire1);
                        return;
                    }
                    else
                    {
                        NpcAI.TryCastAbility(_fire3);
                        return;
                    }
                }
            }
            else if (NpcAI.INpcBaseCreature.ManaController.CurrentMP >= _fire1.ManaCost)
            {
                if (buffAstralFire != null)
                {
                    NpcAI.TryCastAbility(_fire1);
                    return;
                }

                if (buffUmbralIce != null)
                {
                    if (shouldCastThunder3IfPossible)
                    {
                        if (NpcAI.CanTryCast(_thunder3))
                        {
                            NpcAI.TryCastAbility(_thunder3);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (NpcAI.INpcBaseCreature.ManaController.CanSpentAmountOfMana(_blizzard3.ManaCost))
                {
                    NpcAI.TryCastAbility(_blizzard3);
                    return;
                }
                else
                {
                    if (buffAstralFire != null)
                    {
                        NpcAI.TryCastAbility(_transpose);
                        return;
                    }
                    else
                    {
                        NpcAI.TryCastAbility(_blizzard1);
                        return;
                    }
                }
            }
        }
    }
}