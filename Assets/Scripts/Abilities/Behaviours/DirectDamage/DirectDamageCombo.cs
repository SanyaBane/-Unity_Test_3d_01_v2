using Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour
{
    public class DirectDamageCombo : DirectDamage
    {
        public DirectDamageComboSO DirectDamageComboSO => (DirectDamageComboSO) base.AbilityBehaviourSO;

        public int ComboPotency;


        public DirectDamageCombo(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            ComboPotency = DirectDamageComboSO.ComboPotency;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            int potencyToUse = Potency;

            if (ability.AbilitySO.ComboContinuerAbilitySO != null)
            {
                if (ability.PerformCombo)
                {
                    potencyToUse = ComboPotency;
                }
            }
            else
            {
                Debug.LogError($"{nameof(ability)} is not a {nameof(ability.AbilitySO.ComboContinuerAbilitySO)}");
            }

            var damage = DamageInfo.CalculateDamageFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, potencyToUse);

            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetHealth = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Health;
                if (targetHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, damage);
                    targetHealth.TryInflictDamage(damageInfo);
                }
            }
        }
    }
}