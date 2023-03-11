using Assets.Scripts.Abilities.Behaviours.General;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/BuffApplierSource")]
    public class BuffApplierSourceSO : BaseBuffApplierSO
    {
        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new BuffApplierSource(this);
            return ret;
        }
    }
}