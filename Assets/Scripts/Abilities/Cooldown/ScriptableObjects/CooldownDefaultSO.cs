using Assets.Scripts.Abilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Abilities.Cooldown.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/Cooldown/Default")]
    public class CooldownDefaultSO : BaseAbilityCooldownSO
    {
        [SerializeField] private float _defaultCooldown;
        public float DefaultCooldown => _defaultCooldown;

        [SerializeField] private bool _affectsGlobalCooldown = true;
        public bool AffectsGlobalCooldown => _affectsGlobalCooldown;

        public override BaseAbilityCooldown CreateCooldown(Ability ability)
        {
            var cooldownDefault = CooldownDefault.CreateCooldownDefault(ability);
            return cooldownDefault;
        }
    }
}