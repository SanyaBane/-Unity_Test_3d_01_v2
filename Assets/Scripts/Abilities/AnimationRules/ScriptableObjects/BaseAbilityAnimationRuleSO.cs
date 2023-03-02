using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.AnimationRules.ScriptableObjects
{
    public abstract class BaseAbilityAnimationRuleSO : ScriptableObject
    {
        public abstract BaseAbilityAnimationRule CreateAbilityAnimationRule(Ability ability);
    }
}