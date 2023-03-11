using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectHealing : AbilityBehaviour, IBehaviourWithName
    {
        public DirectHealingSO DirectHealingSO => (DirectHealingSO) base.AbilityBehaviourSO;
        
        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public int Potency { get; }
        
        public DirectHealing(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = DirectHealingSO.Name;
            ShareNameWithAbility = DirectHealingSO.ShareNameWithAbility;
            Potency = DirectHealingSO.Potency;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var heal = DamageInfo.CalculateHealFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);
            
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetHealth = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Health;
                if (targetHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, heal);
                    targetHealth.TryInflictHealing(damageInfo);
                }
            }
        }
    }
}