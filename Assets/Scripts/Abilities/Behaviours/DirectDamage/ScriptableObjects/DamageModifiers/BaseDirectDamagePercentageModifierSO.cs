using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.DamageModifiers.ScriptableObjects
{
    public abstract class BaseDirectDamagePercentageModifierSO : ScriptableObject
    {
        public abstract float? GetIncreasePercentage(IAbilityParameters iAbilityParameters);
    }
}