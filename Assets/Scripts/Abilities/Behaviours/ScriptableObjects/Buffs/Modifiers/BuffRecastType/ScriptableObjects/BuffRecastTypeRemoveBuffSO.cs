using Assets.Scripts.Buffs;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects
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