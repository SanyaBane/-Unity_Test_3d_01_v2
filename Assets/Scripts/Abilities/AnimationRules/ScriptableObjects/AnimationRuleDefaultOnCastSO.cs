using Assets.Scripts.Abilities.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.AnimationRules.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AnimationRules/DefaultOnCast")]
    public class AnimationRuleDefaultOnCastSO : BaseAbilityAnimationRuleSO
    {
        [SerializeField] private string _animationId = "";
        public string AnimationId => _animationId;

        public override BaseAbilityAnimationRule CreateAbilityAnimationRule(Ability ability)
        {
            var animationRuleOnCastFinish = new AnimationRuleDefaultOnCast(ability);
            return animationRuleOnCastFinish;
        }
    }
}