using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects
{
    public abstract class BaseBuffRecastTypeSO : ScriptableObject
    {
        public abstract BaseBuffRecastType CreateBaseBuffRecastType();
    }
}