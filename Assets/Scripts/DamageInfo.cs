using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class DamageInfo
    {
        public Ability Ability { get; }
        public IAbilityParameters IAbilityParameters { get; }
        public IBehaviourWithName IBehaviourWithName { get; }
        public int Damage { get; }

        public float BonusThreatPercentage { get; set; }
        public int BonusThreatValue { get; set; }

        public DamageInfo(IAbilityParameters abilityParameters, IBehaviourWithName iBehaviourWithName, int damage)
        {
            IAbilityParameters = abilityParameters;
            IBehaviourWithName = iBehaviourWithName;
            Damage = damage;
        }

        public DamageInfo(Ability ability, IAbilityParameters iAbilityParameters, IBehaviourWithName iBehaviourWithName, int damage) : this(iAbilityParameters, iBehaviourWithName, damage)
        {
            Ability = ability;
        }

        public int CalculateThreat(int damage)
        {
            int outputThreat = damage;
            
            if (outputThreat == 0)
                return outputThreat; // Optimization? No reason to calculate percentage, if threat is zero

            var sourceCreatureStatsController = IAbilityParameters.DefaultAbilityParameters.Source.StatsController;
            float increaseOutputThreatPercentage = sourceCreatureStatsController.GetIncreasedOutputThreatPercentage();
            if (increaseOutputThreatPercentage > 0)
            {
                outputThreat += Mathf.CeilToInt(outputThreat * (increaseOutputThreatPercentage / 100));
            }

            if (BonusThreatPercentage > 0)
            {
                outputThreat += Mathf.CeilToInt(outputThreat * (BonusThreatPercentage / 100));
            }

            outputThreat += BonusThreatValue;

            return outputThreat;
        }
        
        public static int CalculateDamageFromPotency(IBaseCreature caster, int potency)
        {
            int damageValue = potency;

            var sourceCreatureStatsController = caster.StatsController;
            float increaseDamagePercentageSum = sourceCreatureStatsController.GetIncreaseDamagePercentage();
            if (increaseDamagePercentageSum != 0)
                damageValue += Mathf.CeilToInt((float) damageValue / 100 * increaseDamagePercentageSum);


            if (damageValue < 0)
                damageValue = 0;

            return damageValue;
        }

        public static int CalculateHealFromPotency(IBaseCreature caster, int potency)
        {
            int damageValue = potency;

            if (damageValue < 0)
                damageValue = 0;

            return damageValue;
        }
    }
}