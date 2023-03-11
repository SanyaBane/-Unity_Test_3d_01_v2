using Assets.Scripts.Abilities.Behaviours.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects
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
