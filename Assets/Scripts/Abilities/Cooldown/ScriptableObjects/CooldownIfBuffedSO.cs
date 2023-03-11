using System.Collections.Generic;
using Assets.Scripts.Abilities.General;
using UnityEngine;

namespace Abilities.Cooldown.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/Cooldown/IfBuffed")]
    public class CooldownIfBuffedSO : BaseAbilityCooldownSO
    {
        public float CooldownWhenBuffed = 0;
        public float CooldownWhenNotBuffed = 0;
        
        // public bool AffectsGlobalCooldownWhenNotBuffed;
        public bool AffectsGlobalCooldown;
        
        public List<string> BuffIds = new List<string>();
        
        public override BaseAbilityCooldown CreateCooldown(Ability ability)
        {
            var cooldownDefault = new CooldownIfBuffed(ability);
            return cooldownDefault;
        }
    }
}