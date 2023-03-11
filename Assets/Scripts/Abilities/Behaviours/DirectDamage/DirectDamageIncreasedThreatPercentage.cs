using Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour
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