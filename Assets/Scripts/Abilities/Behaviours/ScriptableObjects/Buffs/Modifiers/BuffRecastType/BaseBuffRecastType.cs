using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType
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