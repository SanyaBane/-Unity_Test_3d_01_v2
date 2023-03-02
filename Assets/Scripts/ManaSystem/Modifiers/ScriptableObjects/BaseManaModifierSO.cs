using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects
{
    public abstract class BaseManaModifierSO : ScriptableObject
    {
        private const short DEFAULT_PRIORITY = 50;

        public short Priority = DEFAULT_PRIORITY;

        public enum EManaModifierType
        {
            OnExternalSourceChange,
            OnGameTick,
        }

        public EManaModifierType ManaModifierType = EManaModifierType.OnExternalSourceChange;

        public abstract BaseManaModifier CreateBaseManaModifier(IBaseCreature iBaseCreature);
    }
}