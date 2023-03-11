using Assets.Scripts.Creatures;
using System.Collections.Generic;
using System.Diagnostics;
using Abilities.Cooldown;
using Assets.Scripts.Abilities.AnimationRules;
using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.General.ScriptableObjects;
using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Abilities.Modifiers;
using Assets.Scripts.Abilities.OnCreateCustomLogic;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Abilities.General
{
    [DebuggerDisplay("Id: {AbilitySO.Id}; Name: {AbilitySO.Name};")]
    public abstract class Ability
    {
        public int ManaCost;

        public float CastTime; // TODO implement cast speed
        public List<CastSpeedModifier> CastSpeedModifiers = new List<CastSpeedModifier>();
        
        public float DelayAfterFinishCast;
        public int Order;
        public bool ResetAutoAttackTimerAfterCastFinish;
        public bool BreaksOthersComboChain;
        public bool CanDisplayErrorMessages;

        public IAbilitiesController IAbilitiesController { get; private set; }
        public AbilitySO AbilitySO { get; private set; }

        public bool PerformCombo { get; set; } = false;

        public float TimeUntilComboActionAvailable { get; private set; } = 0f;

        // public bool IsCastInstant => CastTime == 0;
        public bool IsCastInstant(IBaseCreature baseCreature)
        {
            // TODO Check if something has an effect on cast time. "QuickCast" buff, etc.
            
            if (CastTime == 0)
                return true;

            return false;
        }

        public BaseAbilityCooldown AbilityCooldown { get; }

        private BaseAbilityAnimationRule _abilityAnimationRule;

        private readonly List<AbilityBehaviour> _abilityBehavioursBeforeDelay = new List<AbilityBehaviour>();
        private readonly List<AbilityBehaviour> _abilityBehaviours = new List<AbilityBehaviour>();

        private readonly List<BaseOnCreateCustomLogic> _listOnCreateCustomLogic = new List<BaseOnCreateCustomLogic>();

        protected Ability(AbilitySO abilitySO, IAbilitiesController iAbilitiesController)
        {
            AbilitySO = abilitySO;
            IAbilitiesController = iAbilitiesController;

            if (AbilitySO == null)
                return;

            ManaCost = AbilitySO.InitialManaCost;
            CastTime = AbilitySO.InitialCastTime;
            DelayAfterFinishCast = AbilitySO.DelayAfterFinishCast;
            Order = AbilitySO.Order;
            ResetAutoAttackTimerAfterCastFinish = AbilitySO.ResetAutoAttackTimerAfterCastFinish;
            BreaksOthersComboChain = AbilitySO.BreaksOthersComboChain;
            CanDisplayErrorMessages = AbilitySO.CanDisplayErrorMessages;

            if (AbilitySO.AbilityAnimationRuleSO != null)
            {
                _abilityAnimationRule = AbilitySO.AbilityAnimationRuleSO.CreateAbilityAnimationRule(this);
            }
            // else
            // {
            //     UnityEngine.Debug.LogError($"{nameof(AbilitySO.AbilityAnimationRuleSO)} == null for AbilitySO.Id = '{AbilitySO.Id}'");
            // }


            if (AbilitySO.CooldownSO != null)
            {
                AbilityCooldown = AbilitySO.CooldownSO.CreateCooldown(this);
            }
            else
            {
                AbilityCooldown = CooldownDefault.CreateCooldownDefault(this);
            }

            foreach (var abilityBehaviourSO in AbilitySO.AbilityBehavioursSO)
            {
                var abilityBehaviour = abilityBehaviourSO.CreateAbilityBehaviour();
                _abilityBehaviours.Add(abilityBehaviour);
            }

            foreach (var abilityBehaviourBeforeDelaySO in AbilitySO.AbilityBehavioursBeforeDelaySO)
            {
                var abilityBehaviourBeforeDelay = abilityBehaviourBeforeDelaySO.CreateAbilityBehaviour();
                _abilityBehavioursBeforeDelay.Add(abilityBehaviourBeforeDelay);
            }

            ProcessOnCreateCustomLogicObjects();
        }

        public void CalculateAbilityBehavioursBeforeDelay(Ability ability, IAbilityParameters iAbilityParameters)
        {
            foreach (var abilityBehaviour in _abilityBehaviours)
            {
                if (abilityBehaviour is IBehaviourSetupBeforeDelay iBehaviourSetupBeforeDelay)
                {
                    iBehaviourSetupBeforeDelay.SetupBeforeDelay(ability, iAbilityParameters);
                }
            }

            foreach (var abilityBehaviourBeforeDelay in _abilityBehavioursBeforeDelay)
            {
                if (abilityBehaviourBeforeDelay is IBehaviourSetupBeforeDelay iBehaviourSetupBeforeDelay)
                {
                    iBehaviourSetupBeforeDelay.SetupBeforeDelay(ability, iAbilityParameters);
                }
            }
        }

        private void ProcessOnCreateCustomLogicObjects()
        {
            foreach (var onCreateCustomLogic in AbilitySO.ListOnCreateCustomLogicSO)
            {
                var obj = onCreateCustomLogic.CreateOnCreateCustomLogicObject(IAbilitiesController, this);
                _listOnCreateCustomLogic.Add(obj);
            }
        }

        public void UpdateAbilityTimers()
        {
            #region Cooldown

            AbilityCooldown.TickCooldown();

            #endregion

            #region Combo

            if (TimeUntilComboActionAvailable > 0)
            {
                TimeUntilComboActionAvailable -= Time.deltaTime;

                if (TimeUntilComboActionAvailable < 0)
                    TimeUntilComboActionAvailable = 0;
            }

            #endregion
        }

        public void SetOnCooldown()
        {
            AbilityCooldown.SetAbilityOnCooldown();
        }

        public void SetComboTime()
        {
            TimeUntilComboActionAvailable = Constants.TIME_COMBO_ACTION_AVAILABLE;
        }

        public void ResetComboTime()
        {
            TimeUntilComboActionAvailable = 0;
        }

        public bool IsAvailable(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            foreach (var customAvailability in AbilitySO.CustomAvailabilitiesSO)
            {
                if (customAvailability.IsCanStartCast(iAbilityParameters) == false)
                {
                    if (displayErrorMessage)
                        DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_NOT_AVAILABLE, iAbilityParameters);

                    return false;
                }
            }

            return true;
        }

        public virtual bool IsAbilityCanStartCast(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            if (AbilityCooldown.TimeUntilCooldownFinish > 0)
            {
                if (displayErrorMessage)
                    DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_COOLDOWN, iAbilityParameters);

                return false;
            }

            if (IsAbilityCanStartOrFinishCast(iAbilityParameters, displayErrorMessage) == false)
                return false;

            return true;
        }

        public virtual bool IsAbilityCanFinishCast(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            if (IsAbilityCanStartOrFinishCast(iAbilityParameters, displayErrorMessage) == false)
                return false;

            return true;
        }

        public virtual bool IsAbilityCanStartOrFinishCast(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Source is PlayerCreature playerCreature)
            {
                if (IsPlayerAbilityCanStartOrFinishCast(playerCreature) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsPlayerAbilityCanStartOrFinishCast(PlayerCreature playerCreature)
        {
            if (playerCreature.IsMoving && CastTime > 0)
            {
                DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHILE_MOVING);
                return false;
            }

            if (playerCreature.IPlayerController.IsTurningByKeyboard && CastTime > 0)
            {
                DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHILE_TURNING);
                return false;
            }

            return true;
        }

        public void DisplayErrorMessage(string errorMessage)
        {
            if (CanDisplayErrorMessages)
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage(errorMessage);
        }

        public void DisplayErrorMessage(string errorMessage, IAbilityParameters iAbilityParameters)
        {
            if (CanDisplayErrorMessages && iAbilityParameters.DefaultAbilityParameters.Source is PlayerCreature)
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage(errorMessage);
        }

        public void ApplyAbilityBehavioursBeforeDelay(IAbilityParameters iAbilityParameters)
        {
            foreach (var abilityBehaviour in _abilityBehavioursBeforeDelay)
            {
                if (abilityBehaviour == null)
                    continue;

                if (abilityBehaviour.CanApplyBehaviour(this, iAbilityParameters))
                {
                    abilityBehaviour.ApplyBehaviour(this, iAbilityParameters);
                }
            }
        }

        public void ApplyAbilityBehaviours(IAbilityParameters iAbilityParameters)
        {
            foreach (var abilityBehaviour in _abilityBehaviours)
            {
                if (abilityBehaviour == null)
                    continue;

                if (abilityBehaviour.CanApplyBehaviour(this, iAbilityParameters))
                {
                    abilityBehaviour.ApplyBehaviour(this, iAbilityParameters);
                }
            }
        }

        public virtual IAbilityParameters CreateSpecialAbilityParameters(Ability ability, IAbilityParameters iAbilityParameters)
        {
            return null;
        }

        public abstract void ExecuteAbility(IAbilityParameters iAbilityParameters);

        public virtual AbilityParameters CreateAbilityParameters(IBaseCreature iBaseCreature)
        {
            var abilityParameters = new AbilityParameters()
            {
                DefaultAbilityParameters = new DefaultAbilityParameters(iBaseCreature, iBaseCreature.ITargetable)
            };

            return abilityParameters;
        }

        public virtual void OnCastInterrupted(IAbilityParameters iAbilityParameters)
        {
        }

        public virtual void OnCastStarted(IAbilityParameters iAbilityParameters)
        {
        }

        public virtual void OnCastTicked(IAbilityParameters iAbilityParameters)
        {
        }

        public virtual void OnCastFinished(IAbilityParameters iAbilityParameters)
        {
        }

        public virtual void OnCastFinishedAndExecuted(IAbilityParameters iAbilityParameters)
        {
        }
    }
}