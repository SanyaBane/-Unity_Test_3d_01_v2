using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.DamageModifiers;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectDamageIncreasedThreatPercentage : DirectDamage
    {
        public DirectDamageIncreasedThreatPercentageSO DirectDamageIncreasedThreatPercentageSO => (DirectDamageIncreasedThreatPercentageSO) base.AbilityBehaviourSO;
        
        
        public float BonusThreatPercentage;
        
        public DirectDamageIncreasedThreatPercentage(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            BonusThreatPercentage = DirectDamageIncreasedThreatPercentageSO.BonusThreatPercentage;
        }


        protected override DamageInfo CreateDamageInfo(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var damageInfo = base.CreateDamageInfo(ability, iAbilityParameters);
            
            damageInfo.BonusThreatPercentage = BonusThreatPercentage;
            return damageInfo;
        }
    }
}