using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType
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