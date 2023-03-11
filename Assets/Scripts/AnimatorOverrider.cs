using UnityEngine;

namespace Assets.Scripts
{
    public class AnimatorOverrider : MonoBehaviour
    {
        private Animator _animator;

        public AnimatorOverrideController animatorOverrideController;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            SetAnimationOverrideController(animatorOverrideController);
        }

        public void SetAnimationOverrideController(AnimatorOverrideController animatorOverrideController)
        {
            _animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}
