using Assets.Scripts.Abilities.Behaviours.Buffs.Applier;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Applier
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