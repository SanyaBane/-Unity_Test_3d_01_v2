﻿using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectDamageAOE : AbilityBehaviour, IBehaviourWithName
    {
        public DirectDamageAOESO DirectDamageAOESO => (DirectDamageAOESO) base.AbilityBehaviourSO;
        
        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public int Potency { get; }
        
        public DirectDamageAOE(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = DirectDamageAOESO.Name;
            ShareNameWithAbility = DirectDamageAOESO.ShareNameWithAbility;
            Potency = DirectDamageAOESO.Potency;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var abilityParametersAOE = iAbilityParameters as AbilityParametersAOE;
            
            var damage = DamageInfo.CalculateDamageFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);

            foreach (var target in abilityParametersAOE.Targets)
            {
                var targetHealth = target.Health;
                if (targetHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, damage);
                    targetHealth.TryInflictDamage(damageInfo);
                }
            }
        }
    }
}