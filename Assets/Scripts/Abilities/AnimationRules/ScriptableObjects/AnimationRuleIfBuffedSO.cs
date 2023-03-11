using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Abilities.AnimationRules.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AnimationRules/IfBuffed")]
    public class AnimationRuleIfBuffedSO : AnimationRuleDefaultOnCastSO
    {
        public bool PlayIfBuffExists = false;
        public List<string> BuffIds = new List<string>();

        public override BaseAbilityAnimationRule CreateAbilityAnimationRule(Ability ability)
        {
            var animationRuleOnCastFinish = new AnimationRuleIfBuffed(ability);
            return animationRuleOnCastFinish;
        }
    }
}