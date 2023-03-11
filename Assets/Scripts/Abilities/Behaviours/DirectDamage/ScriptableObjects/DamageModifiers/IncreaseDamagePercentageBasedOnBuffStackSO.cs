using System;
using System.Linq;
using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.DamageModifiers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamage Modifiers/IncreaseDamageBasedOnBuffStack")]
    public class IncreaseDamagePercentageBasedOnBuffStackSO : BaseDirectDamagePercentageModifierSO
    {
        // public const float DEFAULT_INCREASE_DAMAGE_PERCENTAGE = 0; 
        
        public string BuffId;

        [Serializable]
        public class StacksIncreaseDamage
        {
            public int StacksCount;
            public float IncreaseDamagePercentage;
        }

        public StacksIncreaseDamage[] ListStacksIncreaseDamage;

        public override float? GetIncreasePercentage(IAbilityParameters iAbilityParameters)
        {
            var buff = iAbilityParameters.DefaultAbilityParameters.Source.BuffsController.GetBuffById(BuffId);
            if (buff == null)
                return null;

            var match = ListStacksIncreaseDamage.FirstOrDefault(x => x.StacksCount == buff.StacksCount);
            if (match != null)
            {
                return match.IncreaseDamagePercentage;
            }

            return null;
        }
    }
}