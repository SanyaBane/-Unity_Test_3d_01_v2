using Abilities.Cooldown.ScriptableObjects;
using Assets.Scripts.Abilities;

namespace Abilities.Cooldown
{
    public class CooldownIfBuffed : BaseAbilityCooldown
    {
        public new CooldownIfBuffedSO AbilityCooldownSO => (CooldownIfBuffedSO) base.AbilityCooldownSO;

        public float CooldownWhenBuffed = 0;
        public float CooldownWhenNotBuffed = 0;

        // public bool AffectsGlobalCooldownWhenNotBuffed;
        public bool AffectsGlobalCooldown;

        private float _abilityCooldownOnLastCast;
        public override float GetAbilityCooldownOnLastCast => _abilityCooldownOnLastCast;
        
        public override float GetAbilityCooldown
        {
            get
            {
                foreach (var buffId in AbilityCooldownSO.BuffIds)
                {
                    if (Ability.IAbilitiesController.IBaseCreature.BuffsController.GetBuffById(buffId) != null)
                    {
                        return CooldownWhenBuffed;
                    }
                }

                return CooldownWhenNotBuffed;
            }
        }

        public override bool GetAffectsGlobalCooldown => AffectsGlobalCooldown;

        // public override bool GetAffectsGlobalCooldown
        // {
        //     get
        //     {
        //         foreach (var buffId in AbilityCooldownSO.BuffIds)
        //         {
        //             if (Ability.IAbilitiesController.IBaseCreature.BuffsController.GetBuffById(buffId) != null)
        //             {
        //                 return AffectsGlobalCooldownWhenNotBuffed;
        //             }
        //         }
        //
        //         return !AffectsGlobalCooldownWhenNotBuffed;
        //     }
        // }

        public CooldownIfBuffed(Ability ability) : base(ability)
        {
            // if (AbilityCooldownSO != null)
            // {
            CooldownWhenBuffed = AbilityCooldownSO.CooldownWhenBuffed;
            CooldownWhenNotBuffed = AbilityCooldownSO.CooldownWhenNotBuffed;
            // AffectsGlobalCooldownWhenNotBuffed = AbilityCooldownSO.AffectsGlobalCooldownWhenNotBuffed;
            AffectsGlobalCooldown = AbilityCooldownSO.AffectsGlobalCooldown;
            // }
        }
        
        public override void SetAbilityOnCooldown()
        {
            base.SetAbilityOnCooldown();
            
            _abilityCooldownOnLastCast = GetAbilityCooldown;
        }
    }
}