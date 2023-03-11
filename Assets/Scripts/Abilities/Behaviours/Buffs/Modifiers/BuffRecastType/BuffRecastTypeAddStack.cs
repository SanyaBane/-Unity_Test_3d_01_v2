using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType
{
    public class BuffRecastTypeAddStack : BaseBuffRecastType
    {
        public int AddStackValue = 1;
        public int MaxStacks = 3;

        public BuffRecastTypeAddStack(BuffRecastTypeAddStackSO buffRecastTypeAddStackSO) : base(buffRecastTypeAddStackSO)
        {
            if (buffRecastTypeAddStackSO != null)
            {
                AddStackValue = buffRecastTypeAddStackSO.AddStackValue;
                MaxStacks = buffRecastTypeAddStackSO.MaxStacks;
            }
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            ExecuteRecastLogic(buff, AddStackValue, MaxStacks);
        }

        public static void ExecuteRecastLogic(Buff buff, int addStackValue, int maxStacks)
        {
            var stack = buff.StacksCount + addStackValue;
            if (stack > maxStacks)
                stack = maxStacks;

            buff.StacksCount = stack;
        }
    }
}