using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities
{
    public class AbilityPlacebo : Ability
    {
        private AbilityPlaceboSO _abilityPlaceboSO;

        public AbilityPlacebo(AbilitySO abilitySO, IAbilitiesController iAbilitiesController) : base(abilitySO, iAbilitiesController)
        {
            _abilityPlaceboSO = (AbilityPlaceboSO) abilitySO;
        }

        public override void ExecuteAbility(IAbilityParameters iAbilityParameters)
        {
            // do nothing, it is just placebo ability
        }
    }
}