namespace Assets.Scripts.NPC.Tactics
{
    public abstract class BaseAIPeaceTactics
    {
        public NpcAI NpcAI { get; }

        protected BaseAIPeaceTactics(NpcAI npcAI)
        {
            NpcAI = npcAI;
        }

        public virtual void ProcessPeaceTactics()
        {
            // NpcAI.INpcBaseCreature.AutoAttackController.WeaponSheathed = true;
            
            // sheath weapon in peacefull state
            if (NpcAI.INpcBaseCreature.AutoAttackController.WeaponSheathed == false &&
                NpcAI.INpcBaseCreature.AbilitiesController.CastAbilityCoroutineWrapper.IsInProgress == false &&
                NpcAI.INpcBaseCreature.AbilitiesController.IsFinishingCastingAbility == false &&
                NpcAI.INpcBaseCreature.Animator.GetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY) == false &&
                NpcAI.INpcBaseCreature.Animator.GetBool(ConstantsAnimator.ABILITIES_BOOL_CAST_FINAL_ANIMATION_PLAYING) == false
            )
            {
                NpcAI.INpcBaseCreature.AutoAttackController.WeaponSheathed = true;
            }
        }
    }
}