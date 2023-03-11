using Assets.Scripts.Abilities.Behaviours.Buffs.Applier;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Applier
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