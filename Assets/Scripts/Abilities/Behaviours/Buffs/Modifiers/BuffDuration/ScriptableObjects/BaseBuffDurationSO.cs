using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.ScriptableObjects
{
    public abstract class BaseBuffDurationSO : ScriptableObject
    {
        public float InitialDuration = 9;

        public bool IsPermanent = false;

        public abstract BaseBuffDuration CreateBaseBuffDuration();
    }
}