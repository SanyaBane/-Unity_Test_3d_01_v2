using System;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.DamageModifiers;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectDamage : AbilityBehaviour, IBehaviourWithName, IBehaviourSetupBeforeDelay
    {
        public DirectDamageSO DirectDamageSO => (DirectDamageSO) base.AbilityBehaviourSO;

        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public int Potency { get; }
        public BaseDirectDamagePercentageModifierSO[] DirectDamagePercentageModifiers { get; }
        public DirectDamageSO.ECalculationMode CalculationMode { get; }

        private DamageInfo _calculatedBeforeDelayDamageInfo;

        public DirectDamage(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = DirectDamageSO.Name;
            ShareNameWithAbility = DirectDamageSO.ShareNameWithAbility;
            Potency = DirectDamageSO.Potency;
            DirectDamagePercentageModifiers = DirectDamageSO.DirectDamagePercentageModifiers;
            CalculationMode = DirectDamageSO.CalculationMode;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetHealth = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Health;
                if (targetHealth != null)
                {
                    DamageInfo damageInfo;

                    if (IsSetupBeforeDelay())
                    {
                        damageInfo = _calculatedBeforeDelayDamageInfo;
                    }
                    else
                    {
                        damageInfo = CreateDamageInfo(ability, iAbilityParameters);
                    }

                    targetHealth.TryInflictDamage(damageInfo);
                }
            }
        }

        protected virtual DamageInfo CreateDamageInfo(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var damage = DamageInfo.CalculateDamageFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);

            if (DirectDamagePercentageModifiers.Length > 0)
            {
                float increaseDamagePercentage = 0;
                bool atLeastOneModifierTriggered = false;

                foreach (var directDamagePercentageModifier in DirectDamagePercentageModifiers)
                {
                    float? increasePercentage = directDamagePercentageModifier.GetIncreasePercentage(iAbilityParameters);

                    if (increasePercentage != null)
                    {
                        atLeastOneModifierTriggered = true;
                        increaseDamagePercentage += increasePercentage.Value;
                    }
                }

                if (atLeastOneModifierTriggered)
                    damage = Mathf.CeilToInt(damage / 100f * increaseDamagePercentage);
            }

            var damageInfo = new DamageInfo(ability, iAbilityParameters, this, damage);
            return damageInfo;
        }

        public bool IsSetupBeforeDelay()
        {
            return CalculationMode == DirectDamageSO.ECalculationMode.OnCastFinish;
        }

        public void SetupBeforeDelay(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (IsSetupBeforeDelay())
                _calculatedBeforeDelayDamageInfo = CreateDamageInfo(ability, iAbilityParameters);
        }
    }
}