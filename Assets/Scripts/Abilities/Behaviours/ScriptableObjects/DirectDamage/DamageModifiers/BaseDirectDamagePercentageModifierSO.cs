using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.DamageModifiers
{
    public abstract class BaseDirectDamagePercentageModifierSO : ScriptableObject
    {
        public abstract float? GetIncreasePercentage(IAbilityParameters iAbilityParameters);
    }
}