using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/RecastType/UpdateDurationAndAddStack")]
    public class BuffRecastTypeUpdateDurationAndAddStackSO : BaseBuffRecastTypeSO
    {
        public int AddStackValue = 1;
        public int MaxStacks = 3;
        
        public override BaseBuffRecastType CreateBaseBuffRecastType()
        {
            var ret = new BuffRecastTypeUpdateDurationAndAddStack(this);
            return ret;
        }
    }
}