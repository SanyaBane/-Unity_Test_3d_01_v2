using Abilities.Cooldown.ScriptableObjects;
using Assets.Scripts.Abilities;
using UnityEngine;

namespace Abilities.Cooldown
{
    public abstract class BaseAbilityCooldown
    {
        public Ability Ability { get; }

        public BaseAbilityCooldownSO AbilityCooldownSO => Ability.AbilitySO.CooldownSO;

        private float _timeUntilCooldownFinish;
        public float TimeUntilCooldownFinish
        {
            get => _timeUntilCooldownFinish;
            private set
            {
                _timeUntilCooldownFinish = value;

                if (_timeUntilCooldownFinish < 0)
                    _timeUntilCooldownFinish = 0;
            }
        }
        
        public abstract float GetAbilityCooldownOnLastCast { get; }
        public abstract float GetAbilityCooldown { get; }
        public abstract bool GetAffectsGlobalCooldown { get; }
        // public bool AffectsGlobalCooldown = true;

        public BaseAbilityCooldown(Ability ability)
        {
            Ability = ability;

            if (AbilityCooldownSO != null)
            {
                // DefaultCooldown = AbilityCooldownSO.DefaultCooldown;
                // AffectsGlobalCooldown = AbilityCooldownSO.AffectsGlobalCooldown;
            }
        }

        public virtual void SetAbilityOnCooldown()
        {
            TimeUntilCooldownFinish = GetAbilityCooldown;
        }
        
        public void SetOnSpecificCooldown(float specificCooldown)
        {
            TimeUntilCooldownFinish = specificCooldown;
        }
        
        public void ResetCooldown()
        {
            TimeUntilCooldownFinish = 0;
        }

        public void TickCooldown()
        {
            TimeUntilCooldownFinish -= Time.deltaTime;
        }
    }
}