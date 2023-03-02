using Assets.Scripts.Abilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Abilities.Cooldown.ScriptableObjects
{
    public abstract class BaseAbilityCooldownSO : ScriptableObject
    {
        public abstract BaseAbilityCooldown CreateCooldown(Ability ability);
    }
}