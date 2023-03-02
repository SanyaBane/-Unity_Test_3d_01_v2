using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType
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