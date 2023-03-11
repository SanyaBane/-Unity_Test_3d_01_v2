using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;

namespace Assets.Scripts.Abilities.AnimationRules
{
    public class AnimationRuleDefaultOnCast : BaseAbilityAnimationRule
    {
        public new AnimationRuleDefaultOnCastSO AbilityAnimationRuleSO => (AnimationRuleDefaultOnCastSO) base.AbilityAnimationRuleSO;

        public AnimationRuleDefaultOnCast(Ability ability) : base(ability)
        {
            ability.IAbilitiesController.CastStarted += AbilitiesControllerOnCastStarted;
            ability.IAbilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
            ability.IAbilitiesController.CastFinishedAndExecuted += AbilitiesControllerOnCastFinishedAndExecuted;
            ability.IAbilitiesController.CastInterrupted += AbilitiesControllerOnCastInterrupted;
        }

        protected void AbilitiesControllerOnCastInterrupted(AbilitiesController abilitiesController, Ability ability)
        {
            if (Ability != ability)
                return;

            OnCastInterrupted(abilitiesController, ability);
        }

        protected void AbilitiesControllerOnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            if (Ability != ability)
                return;

            OnCastStarted(abilitiesController, ability);
        }

        protected void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            if (Ability != ability)
                return;

            OnCastFinished(abilitiesController, ability);
        }

        protected void AbilitiesControllerOnCastFinishedAndExecuted(AbilitiesController abilitiesController, Ability ability)
        {
            if (Ability != ability)
                return;

            OnCastFinishedAndExecuted(abilitiesController, ability);
        }

        protected virtual void OnCastInterrupted(AbilitiesController abilitiesController, Ability ability)
        {
            abilitiesController.Animator.SetBool(AbilityAnimationRuleSO.AnimationId, false);
        }

        protected virtual void OnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            abilitiesController.Animator.SetBool(AbilityAnimationRuleSO.AnimationId, true);
        }

        protected virtual void OnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            abilitiesController.Animator.SetBool(AbilityAnimationRuleSO.AnimationId, true);
        }

        protected virtual void OnCastFinishedAndExecuted(AbilitiesController abilitiesController, Ability ability)
        {
            if (Ability != ability)
                return;

            bool setCastingAnimationToFalse = true;
            if (abilitiesController.CastAbilityCoroutineWrapper.IsInProgress)
            {
                if (abilitiesController.CastAbilityCoroutineWrapper.Ability == ability)
                {
                    setCastingAnimationToFalse = false;
                }
            }

            if (setCastingAnimationToFalse)
                abilitiesController.Animator.SetBool(AbilityAnimationRuleSO.AnimationId, false);
        }
    }
}