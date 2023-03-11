using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/RecastType/RemoveBuff")]
    public class BuffRecastTypeRemoveBuffSO : BaseBuffRecastTypeSO
    {
        public override BaseBuffRecastType CreateBaseBuffRecastType()
        {
            var ret = new BuffRecastTypeRemoveBuff(this);
            return ret;
        }
    }
}