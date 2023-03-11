using Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour
{
    public class DirectDamageIncreasedThreatValue : DirectDamage
    {
        public DirectDamageIncreasedThreatValueSO DirectDamageIncreasedThreatValueSO => (DirectDamageIncreasedThreatValueSO) base.AbilityBehaviourSO;
        
        
        public int BonusThreatValue;
        
        public DirectDamageIncreasedThreatValue(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            BonusThreatValue = DirectDamageIncreasedThreatValueSO.BonusThreatValue;
        }


        protected override DamageInfo CreateDamageInfo(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var damageInfo = base.CreateDamageInfo(ability, iAbilityParameters);
            
            damageInfo.BonusThreatValue = BonusThreatValue;
            return damageInfo;
        }
    }
}