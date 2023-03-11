using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType
{
    public abstract class BaseBuffRecastType
    {
        public BaseBuffRecastTypeSO BaseBuffRecastTypeSO { get; }
        
        public BaseBuffRecastType(BaseBuffRecastTypeSO baseBuffRecastTypeSO)
        {
            BaseBuffRecastTypeSO = baseBuffRecastTypeSO;
        }
        
        public abstract void ApplyRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff);

        public virtual void ApplyPostRecastLogic(BuffsController buffsController, Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
        }
    }
}