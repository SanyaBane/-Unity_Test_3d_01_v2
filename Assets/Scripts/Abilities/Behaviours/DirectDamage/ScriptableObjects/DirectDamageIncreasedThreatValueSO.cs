using Assets.Scripts.Abilities.Behaviours.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamageIncreasedThreatValue")]
    public class DirectDamageIncreasedThreatValueSO : DirectDamageSO
    {
        public int BonusThreatValue;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectDamageIncreasedThreatValue(this);
            return ret;
        }
    }
}