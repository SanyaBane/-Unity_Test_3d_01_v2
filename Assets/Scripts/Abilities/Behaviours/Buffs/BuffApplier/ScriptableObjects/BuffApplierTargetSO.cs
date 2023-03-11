using Assets.Scripts.Abilities.Behaviours.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects
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