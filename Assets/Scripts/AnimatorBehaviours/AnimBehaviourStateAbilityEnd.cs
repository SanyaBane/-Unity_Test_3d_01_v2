using Assets.Scripts;
using UnityEngine;

namespace AnimatorBehaviours
{
    public class AnimBehaviourStateAbilityEnd : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY, false);
        }
    }
}