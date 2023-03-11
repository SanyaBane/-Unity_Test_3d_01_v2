namespace Assets.Scripts
{
    public class NPCHealth : BaseHealth
    {
        public float TimeBeforeCorpseVanish = 3.0f;

        public override void Die()
        {
            base.Die();

            if (IBaseCreature.Animator != null)
            {
                IBaseCreature.Animator.SetBool(ConstantsAnimator.HEALTH_BOOL_DEATH, true);
                IBaseCreature.Animator.Play(ConstantsAnimator.DEATH_ANIMATION_NAME);
            }

            RaiseOnDeathEvent();

            if (TimeBeforeCorpseVanish >= 0)
            {
                IBaseCreature.DestroyCreature(TimeBeforeCorpseVanish);
            }
        }
    }
}
