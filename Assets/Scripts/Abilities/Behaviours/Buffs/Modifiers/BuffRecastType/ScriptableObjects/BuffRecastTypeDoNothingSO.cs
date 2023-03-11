using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects
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