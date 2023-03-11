using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Interfaces
{
    public interface IBehaviourSetupBeforeDelay
    {
        bool IsSetupBeforeDelay();
        void SetupBeforeDelay(Ability ability, IAbilityParameters iAbilityParameters);
    }
}