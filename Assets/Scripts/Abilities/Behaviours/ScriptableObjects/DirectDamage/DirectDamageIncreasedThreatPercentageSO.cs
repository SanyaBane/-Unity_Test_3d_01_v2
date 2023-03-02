using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamageIncreasedThreatPercentage")]
    public class DirectDamageIncreasedThreatPercentageSO : DirectDamageSO
    {
        public float BonusThreatPercentage;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectDamageIncreasedThreatPercentage(this);
            return ret;
        }
    }
}