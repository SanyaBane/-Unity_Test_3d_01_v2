using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.AnimationRules
{
    public abstract class BaseAbilityAnimationRule
    {
        public Ability Ability { get; }
        
        public BaseAbilityAnimationRuleSO AbilityAnimationRuleSO => Ability.AbilitySO.AbilityAnimationRuleSO;

        public BaseAbilityAnimationRule(Ability ability)
        {
            Ability = ability;
        }
    }
}