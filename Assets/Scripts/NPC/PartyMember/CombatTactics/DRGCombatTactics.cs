namespace Assets.Scripts.NPC.PartyMember.CombatTactics
{
    public class DRGCombatTactics : DamageDealerCombatTactics
    {
        public DRGCombatTactics(NpcAI npcAI) : base(npcAI) { }

        public override void ProcessCombatTactics()
        {
            ExecuteCombatRotation();
        }
        
        protected override void ExecuteCombatRotation()
        {
            if (NpcAI.INpcBaseCreature.ICanSelectTarget.SelectedTarget == null)
                return;

            if (NpcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility)
                return;
            
            if (IsPrioritizeMovingOverDamageDealing)
                return;
        }
    }
}