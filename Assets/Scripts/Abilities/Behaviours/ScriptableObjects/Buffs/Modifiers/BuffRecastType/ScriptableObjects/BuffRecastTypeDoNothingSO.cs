using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/RecastType/DoNothing")]
    public class BuffRecastTypeDoNothingSO : BaseBuffRecastTypeSO
    {
        public override BaseBuffRecastType CreateBaseBuffRecastType()
        {
            var ret = new BuffRecastTypeDoNothing(this);
            return ret;
        }
    }
}