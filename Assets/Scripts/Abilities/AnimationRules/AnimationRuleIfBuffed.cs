using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;

namespace Assets.Scripts.Abilities.AnimationRules
{
    public class AnimationRuleIfBuffed : AnimationRuleDefaultOnCast
    {
        public new AnimationRuleIfBuffedSO AbilityAnimationRuleSO => (AnimationRuleIfBuffedSO) base.AbilityAnimationRuleSO;

        public AnimationRuleIfBuffed(Ability ability) : base(ability)
        {
            ability.IAbilitiesController.CastStarted += AbilitiesControllerOnCastStarted;
            ability.IAbilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
            ability.IAbilitiesController.CastFinishedAndExecuted += AbilitiesControllerOnCastFinishedAndExecuted;
            ability.IAbilitiesController.CastInterrupted += AbilitiesControllerOnCastInterrupted;
        }

        private bool IsBuffsFromListExists(AbilitiesController abilitiesController)
        {
            foreach (var buffId in AbilityAnimationRuleSO.BuffIds)
            {
                if (abilitiesController.IBaseCreature.BuffsController.GetBuffById(buffId) != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            if (IsBuffsFromListExists(abilitiesController) == AbilityAnimationRuleSO.PlayIfBuffExists)
            {
                base.OnCastStarted(abilitiesController, ability);
            }
        }

        protected override void OnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            if (IsBuffsFromListExists(abilitiesController) == AbilityAnimationRuleSO.PlayIfBuffExists)
            {
                base.OnCastFinished(abilitiesController, ability);
            }
        }
    }
}