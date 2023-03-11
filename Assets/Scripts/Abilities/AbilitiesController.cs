using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Factions;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class AbilitiesController : MonoBehaviour, IAbilitiesController
    {
        [SerializeField] private float _baseGlobalCooldown = Constants.GLOBAL_COOLDOWN_DEFAULT;
        public float BaseGlobalCooldown => _baseGlobalCooldown;

        public bool IsCastingAbility => CastAbilityCoroutineWrapper.IsInProgress && CastAbilityCoroutineWrapper.CurrentlyCastedTime > 0;
        public bool IsFinishingCastingAbility { get; private set; }
        public bool IsCastingOrFinishingCastingAbility => IsCastingAbility || IsFinishingCastingAbility;

        public CastAbilityCoroutineWrapper CastAbilityCoroutineWrapper = new CastAbilityCoroutineWrapper();

        public IBaseCreature IBaseCreature { get; private set; }

        protected AutoAttackController _autoAttack => IBaseCreature.AutoAttackController;
        protected BaseHealth _baseHealth => IBaseCreature.Health;

        public Animator Animator => IBaseCreature.Animator;

        [Header("General")]
        [SerializeField] private List<AbilitySO> _AbilitySOs = new List<AbilitySO>();

        private List<Ability> Abilities { get; } = new List<Ability>();

        public IEnumerable<Ability> GetAbilities()
        {
            return Abilities.AsReadOnly();
        }

        public Ability GetAbilityById(string abilityId)
        {
            var ability = this.Abilities.FirstOrDefault(x => x.AbilitySO.Id == abilityId);
            if (ability == null)
                Debug.LogError($"Ability with ID '{abilityId}' not found.");

            return ability;
        }

        public float TimeUntilGlobalCooldownFinish { get; private set; }

        public event Action<AbilitiesController, Ability> CastStarted;
        public event Action<AbilitiesController, Ability> CastFinished;
        public event Action<AbilitiesController, Ability> CastFinishedAndExecuted;
        public event Action<AbilitiesController, Ability, float> CastTicked;
        public event Action<AbilitiesController, Ability> CastInterrupted;

        // public event Action<AbilitiesController, Ability> SetCastingAnimationToFalse;

        [Header("Debug (readonly)")]
        [SerializeField] [Range(0, 1)]
        private float _castProgressDisplay = 0f;

        public void RaiseCastInterrupted(Ability ability, IAbilityParameters iAbilityParameters)
        {
            _castProgressDisplay = 0;

            ability.OnCastInterrupted(iAbilityParameters);

            CastInterrupted?.Invoke(this, ability);
        }

        public void RaiseCastStart(Ability ability, IAbilityParameters iAbilityParameters)
        {
            ability.OnCastStarted(iAbilityParameters);

            CastStarted?.Invoke(this, ability);
        }

        public void RaiseCastTick(Ability ability, float currentlyCastedTime, IAbilityParameters iAbilityParameters)
        {
            float currentlyCastedPercentage = currentlyCastedTime * 100 / ability.CastTime;
            _castProgressDisplay = currentlyCastedPercentage / 100;

            ability.OnCastTicked(iAbilityParameters);

            CastTicked?.Invoke(this, ability, currentlyCastedTime);
        }

        public void RaiseCastFinished(Ability ability, IAbilityParameters iAbilityParameters)
        {
            _castProgressDisplay = 0;

            ability.OnCastFinished(iAbilityParameters);

            CastFinished?.Invoke(this, ability);
        }

        public void RaiseCastFinishedAndExecuted(Ability ability, IAbilityParameters iAbilityParameters)
        {
            ability.OnCastFinishedAndExecuted(iAbilityParameters);

            CastFinishedAndExecuted?.Invoke(this, ability);
        }


        private void Awake()
        {
            var creatureInfoContainer = GetComponentInParent<CreatureInfoContainer>();

            IBaseCreature = creatureInfoContainer.BaseCreature;
            if (IBaseCreature == null)
                Debug.LogError($"{nameof(IBaseCreature)} == null");

            IBaseCreature.BeforeCreatureDestroy += OnBeforeCreatureDestroy;

            CreateAbilitiesFromSO();
        }

        private void Update()
        {
            #region GlobalCooldown

            if (TimeUntilGlobalCooldownFinish > 0)
            {
                TimeUntilGlobalCooldownFinish -= Time.deltaTime;

                if (TimeUntilGlobalCooldownFinish < 0)
                    TimeUntilGlobalCooldownFinish = 0;
            }

            #endregion

            for (int i = 0; i < Abilities.Count; i++)
            {
                var ability = Abilities[i];
                ability.UpdateAbilityTimers();
            }
        }

        public void SetGlobalCooldown()
        {
            this.TimeUntilGlobalCooldownFinish = BaseGlobalCooldown;
        }

        public void ResetGlobalCooldown()
        {
            this.TimeUntilGlobalCooldownFinish = 0;
        }

        private void CreateAbilitiesFromSO()
        {
            foreach (var abilitySO in _AbilitySOs)
            {
                if (abilitySO == null)
                    continue;

                Ability newAbility = abilitySO.CreateAbility(this);
                Abilities.Add(newAbility);
            }
        }

        private void SetComboTimeForContinuers(AbilitySO comboContinuerAbilitySO)
        {
            var comboContinuerAbilities = this.Abilities
                .Where(x => x.AbilitySO.ComboContinuerAbilitySO != null && x.AbilitySO.ComboContinuerAbilitySO == comboContinuerAbilitySO);

            foreach (var abil in comboContinuerAbilities)
            {
                abil.SetComboTime();
            }
        }

        private void ResetComboTimeForSpecificAbilities(List<Ability> abilities)
        {
            foreach (var ability in abilities)
            {
                ability.ResetComboTime();
            }
        }

        private void ResetComboTimeForAllAbilities()
        {
            var allComboContinuers = this.Abilities
                .Where(x => x.AbilitySO.ComboContinuerAbilitySO != null)
                .ToList();

            ResetComboTimeForSpecificAbilities(allComboContinuers);
        }

        private void ResetComboTime(AbilitySO comboContinuerAbilitySO)
        {
            var matchedComboContinuers = this.Abilities
                .Where(x => x.AbilitySO.ComboContinuerAbilitySO != null && x.AbilitySO.ComboContinuerAbilitySO == comboContinuerAbilitySO)
                .ToList();

            ResetComboTimeForSpecificAbilities(matchedComboContinuers);
        }

        public void TryStartCast(Ability ability)
        {
            var abilityParameters = ability.CreateAbilityParameters(IBaseCreature);

            if (CastAbilityCoroutineWrapper.IsInProgress)
            {
                ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHILE_CASTING_SOMETHING_ELSE, abilityParameters);
                return;
            }

            if (IsCanStartCast(ability, abilityParameters) == false)
                return;

            StartCast(ability, abilityParameters);
        }

        private bool IsCanStartCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (_baseHealth.IsAlive == false)
            {
                ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHEN_DEAD, iAbilityParameters);
                return false;
            }

            if (ability.IsAvailable(iAbilityParameters, true) == false)
                return false;

            if (ability.AbilitySO != null)
            {
                if (ability.AbilityCooldown.GetAffectsGlobalCooldown && TimeUntilGlobalCooldownFinish > 0)
                {
                    ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_GLOBAL_COOLDOWN, iAbilityParameters);
                    return false;
                }
            }

            if (IsCanStartOrFinishCast(ability, iAbilityParameters) == false)
                return false;

            if (ability.IsAbilityCanStartCast(iAbilityParameters, true) == false)
                return false;

            return true;
        }

        public bool IsCanFinishCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (ability.IsAbilityCanFinishCast(iAbilityParameters, true) == false)
                return false;

            if (IsCanStartOrFinishCast(ability, iAbilityParameters) == false)
                return false;

            return true;
        }

        public bool IsCanStartOrFinishCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (ability.ManaCost > 0)
            {
                if (iAbilityParameters.DefaultAbilityParameters.Source.ManaController.CanSpentAmountOfMana(ability.ManaCost) == false)
                {
                    ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_NOT_ENOUGH_MANA, iAbilityParameters);
                    return false;
                }
            }

            return true;
        }

        public bool IsCanContinueCasting(Ability ability, IAbilityParameters iAbilityParameters, bool canContinueCastingFirstCheck)
        {
            if (_baseHealth.IsAlive == false)
            {
                ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHEN_DEAD, iAbilityParameters);
                return false;
            }

            if (IBaseCreature is PlayerCreature playerCreature)
            {
                if (playerCreature.IsMoving)
                {
                    ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHILE_MOVING, iAbilityParameters);
                    return false;
                }

                if (playerCreature.IsTurning && canContinueCastingFirstCheck == false)
                {
                    ability.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHILE_MOVING, iAbilityParameters);
                    return false;
                }
            }

            // manually cancel cast
            if (GameManager.Instance.InputController_WoW.CommandCancelCast)
            {
                return false;
            }

            return true;
        }

        private void InstantRotationToTarget(IAbilityParameters iAbilityParameters)
        {
            // instant rotate to target
            var directionToTarget = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetGroundedPosition() - IBaseCreature.GetRootObjectTransform().position;
            var directionToTargetWithoutY = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            if (directionToTargetWithoutY != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(directionToTargetWithoutY);

                IBaseCreature.GetRootObjectTransform().rotation = lookRotation;
            }
        }
        
        private void StartCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            //Debug.Log($"Starting cast skill {ability.Name}");

            // if (IsCanStartCast(ability, iAbilityParameters) == false)
            //     return;

            if (ability is AbilityTarget abilityTarget)
            {
                if (!abilityTarget.TargetIsAlwaysSelf)
                {
                    var relationWithSelectedTarget = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Faction.GetRelationWith(iAbilityParameters.DefaultAbilityParameters.Source.Faction);

                    // If ability is of "agressive" type - turn on AutoAttack 
                    if (relationWithSelectedTarget <= EFactionRelation.Neutral &&
                        abilityTarget.CanCastOn.HasFlag(EAbilityAffects.Enemies))
                    {
                        _autoAttack.EnableAutoAttackMode();
                    }
                }

                if (ShouldRotateToTargetOfCast(ability, iAbilityParameters) == ERotateOnCast.RotateInstantly)
                {
                    InstantRotationToTarget(iAbilityParameters);
                }
            }

            if (_autoAttack.WeaponSheathed)
                _autoAttack.WeaponSheathed = false;

            ProcessCooldowns(ability);

            CastAbilityCoroutineWrapper = new CastAbilityCoroutineWrapper(this, this, ability, iAbilityParameters);
            CastAbilityCoroutineWrapper.StartWrapperCoroutine();
        }

        public ERotateOnCast ShouldRotateToTargetOfCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (ability is AbilityTarget abilityTarget
                && abilityTarget.IsLookOnTarget
                && iAbilityParameters.DefaultAbilityParameters.IsTargetSameAsSource == false)
            {
                switch (IBaseCreature)
                {
                    case PlayerCreature _:
                        return ERotateOnCast.RotateInstantly;
                    case NpcBaseCreature _ when abilityTarget.IsCastInstant(IBaseCreature) == false:
                        return ERotateOnCast.RotateUsingRotationSpeed;
                }
            }
            
            return ERotateOnCast.DoNotRotate;
        }

        private void ProcessCooldowns(Ability ability)
        {
            if (ability.AbilityCooldown.GetAffectsGlobalCooldown)
            {
                this.SetGlobalCooldown();
            }

            ability.SetOnCooldown();
        }

        public IEnumerator FinishCastSuccessfully(Ability ability, IAbilityParameters iAbilityParameters, CastAbilityCoroutineWrapper currentCastAbilityCoroutineWrapper)
        {
            currentCastAbilityCoroutineWrapper.DestroyEllipseProjection();
            
            if (ability.ResetAutoAttackTimerAfterCastFinish)
            {
                _autoAttack.AbilityAutoAttack.SetOnCooldown();
            }

            RaiseCastFinished(ability, iAbilityParameters);

            Animator.SetTrigger(ConstantsAnimator.ABILITIES_TRIGGER_CAST_FINISHED);

            IsFinishingCastingAbility = true;

            Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CASTING_1_SUCCESS, true);

            // TODO fix animator (WAR 3rd skill) (repeated click on same skill breaks animation - old anim still playing, while it should be already new)
            var usingAbility = Animator.GetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY);
            if (usingAbility)
            {
                // Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY, false);
                Animator.SetTrigger(ConstantsAnimator.ABILITIES_TRIGGER_CANCEL_ABILITY_ANIMATION);
                Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_TRANSFER_ABILITY_USAGE, true);
            }

            ability.CalculateAbilityBehavioursBeforeDelay(ability, iAbilityParameters);

            ProcessComboLogic(ability);

            var specialAbilityParameters = ability.CreateSpecialAbilityParameters(ability, iAbilityParameters);
            // swap if SpecialAbilityParameters implemented
            if (specialAbilityParameters != null)
                iAbilityParameters = specialAbilityParameters;

            if (ability.ManaCost > 0)
            {
                iAbilityParameters.DefaultAbilityParameters.Source.ManaController.SpentAmountOfMana(ability.ManaCost);
            }

            ability.ApplyAbilityBehavioursBeforeDelay(iAbilityParameters);

            if (ability.DelayAfterFinishCast != 0)
            {
                yield return new WaitForSeconds(ability.DelayAfterFinishCast);
            }

            Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CASTING_1_SUCCESS, false);

            IsFinishingCastingAbility = false;

            if (currentCastAbilityCoroutineWrapper.StopCoroutineFlag)
            {
                yield break;
            }

            ability.ExecuteAbility(iAbilityParameters);

            RaiseCastFinishedAndExecuted(ability, iAbilityParameters);
        }

        private void ProcessComboLogic(Ability ability)
        {
            if (ability.AbilitySO.ComboContinuerAbilitySO != null)
            {
                ability.PerformCombo = false;

                if (ability.TimeUntilComboActionAvailable > 0)
                {
                    ability.PerformCombo = true;

                    ResetComboTime(ability.AbilitySO.ComboContinuerAbilitySO); // reset combo timer for this ability
                    SetComboTimeForContinuers(ability.AbilitySO); // дать возможность комбо следующей абилки (при её наличии)
                }
                else if (ability.BreaksOthersComboChain)
                {
                    ResetComboTimeForAllAbilities(); // reset combo timer for all abilities
                }
            }
            else
            {
                if (ability.BreaksOthersComboChain)
                {
                    ResetComboTimeForAllAbilities(); // reset combo timer for all abilities
                }

                SetComboTimeForContinuers(ability.AbilitySO); // дать возможность комбо следующей абилки (при её наличии)
            }
        }

        public void InterruptCast()
        {
            CastAbilityCoroutineWrapper.DestroyEllipseProjection();
            CastAbilityCoroutineWrapper.StopWrapperCoroutine();

            var ability = CastAbilityCoroutineWrapper.Ability;
            var abilityParameters = CastAbilityCoroutineWrapper.IAbilityParameters;

            Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_USING_ABILITY, false);
            Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CAST_FINAL_ANIMATION_PLAYING, false);
            Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CASTING_1_SUCCESS, false);

            if (ability.AbilitySO != null)
            {
                if (ability.AbilityCooldown.GetAffectsGlobalCooldown)
                {
                    ResetGlobalCooldown();
                }
            }

            ability.AbilityCooldown.ResetCooldown();

            RaiseCastInterrupted(ability, abilityParameters);
        }

        private void OnBeforeCreatureDestroy()
        {
            CancelCast();
        }

        // simillar to interrupt, but used before creature is destroyed
        private void CancelCast()
        {
            CastAbilityCoroutineWrapper.DestroyEllipseProjection();
        }
    }
}