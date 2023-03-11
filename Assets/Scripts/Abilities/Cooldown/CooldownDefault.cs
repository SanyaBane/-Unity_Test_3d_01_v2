using Abilities.Cooldown.ScriptableObjects;
using Assets.Scripts.Abilities.General;

namespace Abilities.Cooldown
{
    public class CooldownDefault : BaseAbilityCooldown
    {
        public new CooldownDefaultSO AbilityCooldownSO => (CooldownDefaultSO) base.AbilityCooldownSO;
        
        public float DefaultCooldown;
        public bool AffectsGlobalCooldown;

        private float _abilityCooldownOnLastCast;
        public override float GetAbilityCooldownOnLastCast => DefaultCooldown;
        
        public override float GetAbilityCooldown => DefaultCooldown;
        
        public override bool GetAffectsGlobalCooldown => AffectsGlobalCooldown;
        
        public CooldownDefault(Ability ability) : base(ability)
        {
            if (AbilityCooldownSO != null)
            {
                DefaultCooldown = AbilityCooldownSO.DefaultCooldown;
                AffectsGlobalCooldown = AbilityCooldownSO.AffectsGlobalCooldown;
            }
            else
            {
                DefaultCooldown = 0;
                AffectsGlobalCooldown = true;
            }
        }
        
        public static CooldownDefault CreateCooldownDefault(Ability ability)
        {
            var cooldownDefault = new CooldownDefault(ability);
            return cooldownDefault;
        }

        public override void SetAbilityOnCooldown()
        {
            base.SetAbilityOnCooldown();
            
            _abilityCooldownOnLastCast = GetAbilityCooldown;
        }
    }
}