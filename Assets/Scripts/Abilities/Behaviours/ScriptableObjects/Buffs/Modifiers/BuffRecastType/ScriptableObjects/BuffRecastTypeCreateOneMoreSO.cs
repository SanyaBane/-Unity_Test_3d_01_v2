using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/RecastType/CreateOneMore")]
    public class BuffRecastTypeCreateOneMoreSO : BaseBuffRecastTypeSO
    {
        public override BaseBuffRecastType CreateBaseBuffRecastType()
        {
            var ret = new BuffRecastTypeCreateOneMore(this);
            return ret;
        }
    }
}