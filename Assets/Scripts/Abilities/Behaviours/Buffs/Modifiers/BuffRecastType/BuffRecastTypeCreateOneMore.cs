using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType
{
    public class BuffRecastTypeCreateOneMore : BaseBuffRecastType
    {
        public BuffRecastTypeCreateOneMore(BuffRecastTypeCreateOneMoreSO buffRecastTypeCreateOneMoreSO) : base(buffRecastTypeCreateOneMoreSO)
        {
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            buffsController.CreateAndAddBuff(buff.BaseBuffSO, ability, iAbilityParameters, isRuntimeBuff);
        }
    }
}