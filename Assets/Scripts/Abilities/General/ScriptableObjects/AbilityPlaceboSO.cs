using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/AbilityPlaceboSO")]
    public class AbilityPlaceboSO : AbilitySO
    {
        public override Ability CreateAbility(IAbilitiesController iAbilitiesController)
        {
            var ability = new AbilityPlacebo(this, iAbilitiesController);
            return ability;
        }
    }
}