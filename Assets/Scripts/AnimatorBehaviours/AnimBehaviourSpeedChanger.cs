using UnityEngine;

namespace AnimatorBehaviours
{
    public class AnimBehaviourSpeedChanger : StateMachineBehaviour
    {
        [SerializeField] private AnimationCurve speedAnimationCurve;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var speed = speedAnimationCurve.Evaluate(stateInfo.normalizedTime);

            //Debug.Log($"OnStateUpdate(). stateInfo.normalizedTime: {stateInfo.normalizedTime}; speed: {speed}");
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            animator.speed = speed;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.speed = 1f;
        }
    }
}