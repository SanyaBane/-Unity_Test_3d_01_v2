using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;

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