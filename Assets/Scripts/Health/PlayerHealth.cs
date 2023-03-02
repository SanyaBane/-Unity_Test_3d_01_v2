using Assets.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerHealth : BaseHealth
    {
        private readonly float _deathAnimationDuration = 3.0f;

        [Header("Debug")]
        [SerializeField] private bool _canMoveWhenDead = true;
        public bool CanMoveWhenDead => _canMoveWhenDead;

        public override void Die()
        {
            base.Die();

            if (IBaseCreature.Animator != null)
            {
                IBaseCreature.Animator.SetBool(ConstantsAnimator.HEALTH_BOOL_DEATH, true);
                IBaseCreature.Animator.Play(ConstantsAnimator.DEATH_ANIMATION_NAME);
            }

            RaiseOnDeathEvent();

            PostDeathCoroutineWrapper = new CoroutineWrapper(this, PostDeathCoroutine());
        }

        private CoroutineWrapper PostDeathCoroutineWrapper = new CoroutineWrapper();

        private IEnumerator PostDeathCoroutine()
        {
            yield return new WaitForSeconds(_deathAnimationDuration);

            // разрешить рес и вызвать диалог аля "Вы хотите реснуться за пределами боя?"

            yield break;
        }
    }
}
