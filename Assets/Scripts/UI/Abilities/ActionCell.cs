using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.UI.Abilities
{
    [DebuggerDisplay("AbilityUI: {AbilityUI == null ? null : AbilityUI.Ability.AbilitySO.Name}")]
    public class ActionCell : MonoBehaviour
    {
        [SerializeField] private AbilityUI _abilityUI;
        public AbilityUI AbilityUI => _abilityUI;

        public void SetAbilityUI(AbilityUI abilityUI)
        {
            if (abilityUI == null)
            {
                _abilityUI = null;
                return;
            }

            _abilityUI = abilityUI;
        }
    }
}