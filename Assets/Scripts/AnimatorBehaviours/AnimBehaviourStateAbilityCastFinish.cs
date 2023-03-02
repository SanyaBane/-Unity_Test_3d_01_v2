using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AnimatorBehaviours
{
    public class AnimBehaviourStateAbilityCastFinish : StateMachineBehaviour
    {
        private int _onStateEnterCounter = 0;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log($"AbilityCastFinish - OnStateEnter() _onStateEnterCounter: {_onStateEnterCounter}");
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            _onStateEnterCounter++;

            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CAST_FINAL_ANIMATION_PLAYING, true);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log($"AbilityCastFinish - OnStateExit() _onStateEnterCounter: {_onStateEnterCounter}");
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (_onStateEnterCounter > 1)
            {
                _onStateEnterCounter--;
                return;
            }

            _onStateEnterCounter--;

            animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CAST_FINAL_ANIMATION_PLAYING, false);
        }
    }
}