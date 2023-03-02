using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
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