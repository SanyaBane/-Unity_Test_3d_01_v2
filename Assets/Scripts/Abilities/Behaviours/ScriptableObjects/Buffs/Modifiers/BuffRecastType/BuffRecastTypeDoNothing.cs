using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType
{
    public class BuffRecastTypeDoNothing : BaseBuffRecastType
    {
        public BuffRecastTypeDoNothing(BuffRecastTypeDoNothingSO buffRecastTypeDoNothingSO) : base(buffRecastTypeDoNothingSO)
        {
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            // do nothing
        }
    }
}