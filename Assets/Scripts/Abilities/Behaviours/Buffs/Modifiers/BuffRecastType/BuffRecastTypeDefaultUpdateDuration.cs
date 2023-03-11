using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType
{
    public class BuffRecastTypeDefaultUpdateDuration : BaseBuffRecastType
    {
        public BuffRecastTypeDefaultUpdateDuration(BuffRecastTypeDefaultUpdateDurationSO buffRecastTypeDefaultUpdateDurationSO) : base(buffRecastTypeDefaultUpdateDurationSO)
        {
        }

        public override void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            ExecuteRecastLogic(buffsController, buff);
        }
        
        public override void ApplyPostRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            ExecuteApplyPostRecastLogic(buffsController, buff);
        }
        
        public static void ExecuteRecastLogic(BuffsController buffsController, Buff buff)
        {
            buff.BuffDuration.UpdateDuration(buff);
        }
        
        public static void ExecuteApplyPostRecastLogic(BuffsController buffsController, Buff buff)
        {
            buffsController.RaiseBuffDurationUpdated(buff);
        }
    }
}