using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamageCombo")]
    public class DirectDamageComboSO : DirectDamageSO
    {
        public int ComboPotency;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectDamageCombo(this);
            return ret;
        }
    }
}
