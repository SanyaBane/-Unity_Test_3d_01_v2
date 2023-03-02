using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorBehaviours
{
    public class AnimBehaviourRandomAutoAttack : StateMachineBehaviour
    {
        public int AnimationsCount = 1;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // на текущий момент работает неправильно, ибо срабатывает когда уже заходит внутрь какого-либо State,
            // таким образом рандомное значение применится ни к текущему State, а к следующему

            if (AnimationsCount < 1)
                Debug.LogError($"{nameof(AnimationsCount)} < 1");

            int rnd = 0;
            if (AnimationsCount > 1)
            {
                rnd = Random.Range(0, AnimationsCount);
            }

            animator.SetInteger(ConstantsAnimator.AUTO_ATTACK_RANDOM_INDEX, rnd);
        }
    }
}