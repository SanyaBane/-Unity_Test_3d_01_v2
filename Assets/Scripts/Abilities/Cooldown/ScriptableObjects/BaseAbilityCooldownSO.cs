using Assets.Scripts.Abilities;
using UnityEngine;

namespace Abilities.Cooldown.ScriptableObjects
{
    public abstract class BaseAbilityCooldownSO : ScriptableObject
    {
        public abstract BaseAbilityCooldown CreateCooldown(Ability ability);
    }
}