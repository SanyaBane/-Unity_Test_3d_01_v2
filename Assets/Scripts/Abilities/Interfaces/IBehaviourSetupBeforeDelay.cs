namespace Assets.Scripts.Abilities
{
    public interface IBehaviourSetupBeforeDelay
    {
        bool IsSetupBeforeDelay();
        void SetupBeforeDelay(Ability ability, IAbilityParameters iAbilityParameters);
    }
}