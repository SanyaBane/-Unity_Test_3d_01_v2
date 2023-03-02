using Assets.Scripts.Interfaces;
using Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects;

namespace Assets.Scripts.ManaSystem.Modifiers
{
    public abstract class BaseManaModifier
    {
        public BaseManaModifierSO BaseManaModifierSO { get; }
        
        public IBaseCreature IBaseCreature { get; }

        public short Priority { get; }
        public BaseManaModifierSO.EManaModifierType ManaModifierType { get; }

        protected BaseManaModifier(BaseManaModifierSO baseManaModifierSO, IBaseCreature iBaseCreature)
        {
            BaseManaModifierSO = baseManaModifierSO;
            IBaseCreature = iBaseCreature;

            Priority = baseManaModifierSO.Priority;
            ManaModifierType = baseManaModifierSO.ManaModifierType;
        }

        public abstract int Modify(int manaAmount);
        public abstract bool CanModify();
    }
}