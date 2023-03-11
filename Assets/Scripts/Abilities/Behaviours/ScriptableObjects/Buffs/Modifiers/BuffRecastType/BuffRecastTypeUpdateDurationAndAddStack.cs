using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType
{
    public class BuffRecastTypeUpdateDurationAndAddStack : BaseBuffRecastType
    {
        public int AddStackValue = 1;
        public int MaxStacks = 3;

        public BuffRecastTypeUpdateDurationAndAddStack(BuffRecastTypeUpdateDurationAndAddStackSO buffRecastTypeUpdateDurationAndAddStackSO) : base(buffRecastTypeUpdateDurationAndAddStackSO)
        {
            if (buffRecastTypeUpdateDurationAndAddStackSO != null)
            {
                AddStackValue = buffRecastTypeUpdateDurationAndAddStackSO.AddStackValue;
                MaxStacks = buffRecastTypeUpdateDurationAndAddStackSO.MaxStacks;
            }
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            BuffRecastTypeDefaultUpdateDuration.ExecuteRecastLogic(buffsController, buff);
            BuffRecastTypeAddStack.ExecuteRecastLogic(buff, AddStackValue, MaxStacks);
        }
        
        public override void ApplyPostRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            BuffRecastTypeDefaultUpdateDuration.ExecuteApplyPostRecastLogic(buffsController, buff);
        }
    }
}