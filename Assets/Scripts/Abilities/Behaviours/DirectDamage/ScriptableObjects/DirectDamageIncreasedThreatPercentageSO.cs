using Assets.Scripts.Abilities.Behaviours.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects
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