using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/BuffApplierTarget")]
    public class BuffApplierTargetSO : BaseBuffApplierSO
    {
        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new BuffApplierTarget(this);
            return ret;
        }
    }
}