using Assets.Scripts;
using UnityEngine;

namespace AnimatorBehaviours
{
    public class AnimBehaviourStateAbilityStart : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_TRANSFER_ABILITY_USAGE, false);
            animator.ResetTrigger(ConstantsAnimator.ABILITIES_TRIGGER_CANCEL_ABILITY_ANIMATION);

            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY, true);
        }
    }
}