using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Abilities.Cooldown.ScriptableObjects;
using Abilities.CustomAvailability;
using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;
using Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.ScriptableObjects
{
    public abstract class AbilitySO : ScriptableObject
    {
        [Header("General")]
        public string Id;

        /// <summary>
        /// User-friendly ability name.
        /// </summary>
        public string Name;
        // public string Description;
        public Sprite Icon;

        public int InitialManaCost;
        
        public float InitialCastTime;
        public float DelayAfterFinishCast;

        public BaseAbilityAnimationRuleSO AbilityAnimationRuleSO;
        public BaseAbilityCooldownSO CooldownSO;

        public int Order = 1;

        public bool ResetAutoAttackTimerAfterCastFinish = false;

        public bool BreaksOthersComboChain = true;
        public bool CanDisplayErrorMessages = true;

        public AbilitySO ComboContinuerAbilitySO;

        public List<AbilityBehaviourSO> AbilityBehavioursBeforeDelaySO;
        public List<AbilityBehaviourSO> AbilityBehavioursSO;
        
        public List<BaseCustomAvailabilitySO> CustomAvailabilitiesSO;

        public List<BaseOnCreateCustomLogicSO> ListOnCreateCustomLogicSO;
        
        public abstract Ability CreateAbility(IAbilitiesController iAbilitiesController);
    }
}