using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours.General
{
    public class HealCaster : AbilityBehaviour, IBehaviourWithName
    {
        public HealCasterSO HealCasterSO => (HealCasterSO) base.AbilityBehaviourSO;
        
        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public int Potency { get; }
        
        public HealCaster(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = HealCasterSO.Name;
            ShareNameWithAbility = HealCasterSO.ShareNameWithAbility;
            Potency = HealCasterSO.Potency;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var heal = DamageInfo.CalculateHealFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);
            
            if (iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform() != null)
            {
                var casterHealth = iAbilityParameters.DefaultAbilityParameters.Source.Health;
                if (casterHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, heal);
                    casterHealth.TryInflictHealing(damageInfo);
                }
            }
        }
    }
}