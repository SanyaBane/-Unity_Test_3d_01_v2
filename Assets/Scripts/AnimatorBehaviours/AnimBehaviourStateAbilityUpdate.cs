using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AnimatorBehaviours
{
    public class AnimBehaviourStateAbilityUpdate : StateMachineBehaviour
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY, true);
        }
    }
}