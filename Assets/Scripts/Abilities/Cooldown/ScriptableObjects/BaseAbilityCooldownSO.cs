using Assets.Scripts.Abilities.General;
using UnityEngine;

namespace Abilities.Cooldown.ScriptableObjects
{
    public abstract class BaseAbilityCooldownSO : ScriptableObject
    {
        public abstract BaseAbilityCooldown CreateCooldown(Ability ability);
    }
}