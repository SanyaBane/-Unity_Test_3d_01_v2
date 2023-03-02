using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs
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