using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType
{
    public class BuffRecastTypeRemoveBuff : BaseBuffRecastType
    {
        public BuffRecastTypeRemoveBuff(BuffRecastTypeRemoveBuffSO buffRecastTypeRemoveBuffSO) : base(buffRecastTypeRemoveBuffSO)
        {
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            buffsController.RemoveRuntimeBuff(buff);
        }
    }
}