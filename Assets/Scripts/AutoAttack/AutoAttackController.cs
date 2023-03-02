using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.TargetHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Attachments;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

namespace Assets.Scripts.AutoAttack
{
    [RequireComponent(typeof(TargetHandler))]
    public abstract class AutoAttackController : MonoBehaviour, IAbilitiesController
    {
        #region AutoAttackModeOn

        private bool _AutoAttackModeOn = false;
        public bool AutoAttackModeOn
        {
            get => _AutoAttackModeOn;
            private set
            {
                bool valueIsDifferentFromBefore = _AutoAttackModeOn != value;

                _AutoAttackModeOn = value;

                if (valueIsDifferentFromBefore)
                    AutoAttackModeOnChanged?.Invoke(value);
            }
        }
        public event Action<bool> AutoAttackModeOnChanged;

        public void EnableAutoAttackMode()
        {
            ChangeAutoAttackMode(true);
        }

        public void DisableAutoAttackMode()
        {
            ChangeAutoAttackMode(false);
        }

        private void ChangeAutoAttackMode(bool turnOn)
        {
            if (turnOn && !_health.IsAlive)
            {
                // существо не может включить режим автоатаки, если оно мертво
                return;
            }

            AutoAttackModeOn = turnOn;
        }

        #endregion

        #region WeaponSheathed

        private bool _WeaponSheathed = true;
        public bool WeaponSheathed
        {
            get => _WeaponSheathed;
            set
            {
                bool newValueIsDifferentFromBefore = _WeaponSheathed != value;

                _WeaponSheathed = value;

                if (newValueIsDifferentFromBefore)
                    WeaponSheathedChanged?.Invoke(value);
            }
        }
        public event Action<bool> WeaponSheathedChanged;

        #endregion

        #region IAbilitiesController

        public event Action<AbilitiesController, Ability> CastStarted;
        public event Action<AbilitiesController, Ability> CastFinished;
        public event Action<AbilitiesController, Ability> CastFinishedAndExecuted;
        public event Action<AbilitiesController, Ability, float> CastTicked;
        public event Action<AbilitiesController, Ability> CastInterrupted;

        #endregion

        private IBaseCreature _baseCreature;
        public IBaseCreature IBaseCreature => _baseCreature;

        protected BaseHealth _health;
        private Animator _animator;
        private AbilitiesController _abilitiesController;

        public AbilityAutoAttack AbilityAutoAttack { get; private set; }

        [Header("Debug")]
        public bool DrawAutoAttackRays = false;

        [Header("Debug (readonly)")]
        [SerializeField] private float _debugAutoAttackCooldown;

        [Header("General")]
        public bool CanAutoAttack = true;

        /// <summary>
        /// Can be null. Used to hold weapon in aggressive stance.
        /// </summary>
        public Transform Weapon2HRootBone;

        public Transform Weapon1HRightRootBone;
        public Transform Weapon1HLeftRootBone;
        public Transform ShieldRootBone;

        [SerializeField] private AbilityAutoAttackSO _abilityAutoAttackSO;
        public AbilityAutoAttackSO AbilityAutoAttackSO
        {
            get => _abilityAutoAttackSO;
            private set { _abilityAutoAttackSO = value; }
        }

        private CoroutineWrapper _attemptToStartAutoAttackCoroutineWrapper = new CoroutineWrapper();

        public bool AttemptingToAutoAttack { get; private set; } = false;

        public void ChangeAutoAttackSO(AbilityAutoAttackSO abilityAutoAttackSO)
        {
            AbilityAutoAttackSO = abilityAutoAttackSO;
            UpdateAutoAttackFromSO();
        }

        protected void UpdateAutoAttackFromSO()
        {
            if (AbilityAutoAttackSO != null)
            {
                AbilityAutoAttack = (AbilityAutoAttack) AbilityAutoAttackSO.CreateAbility(this);
            }
            // _autoAttackAbility.Setup(AutoAttackSO);

            // _autoAttackAbility.SetOnCooldown();
        }

        protected virtual void Start()
        {
            var creatureInfoContainer = GetComponentInParent<CreatureInfoContainer>();

            _baseCreature = creatureInfoContainer.BaseCreature;
            if (_baseCreature == null)
                Debug.LogError($"{nameof(_baseCreature)} == null");

            _health = _baseCreature.Health;
            if (_health == null)
                Debug.LogError($"{nameof(_health)} == null");

            _abilitiesController = _baseCreature.AbilitiesController;

            _animator = _baseCreature.Animator;

            SetWeaponPosSheathed();

            WeaponSheathedChanged += AutoAttack_WeaponSheathedChanged;
            AutoAttackModeOnChanged += AutoAttack_AutoAttackModeOnChanged;
        }

        private void AutoAttack_AutoAttackModeOnChanged(bool obj)
        {
            // if "Auto attack", disable "Weapon sheathed"
            if (_AutoAttackModeOn == true)
                WeaponSheathed = false;
        }

        private void SetWeaponPosSheathed()
        {
            var attachmentsController_WAR = _baseCreature.AttachmentsController as AttachmentsController_WAR;

            Vector3 weapon_2H_SheathedLocalOffset = new Vector3(-0.015f, 0.0006f, 0.005f);
            Vector3 weapon_2H_SheathedLocalRotation = new Vector3(-13, 170, -278);

            Vector3 weapon_1H_Right_SheathedLocalOffset = new Vector3(0.014f, 0.0f, 0.0f);
            Vector3 weapon_1H_Right_SheathedLocalRotation = new Vector3(0f, 2, -273);

            Vector3 weapon1HLeftSheathedLocalOffset = new Vector3(-0.02f, 0.0015f, 0.005f);
            Vector3 weapon1HLeftSheathedLocalRotation = new Vector3(0, 0, -128);

            Vector3 shieldSheathedLocalOffset = new Vector3(-0.02f, 0, 0);
            Vector3 shieldSheathedLocalRotation = new Vector3(0, 180, 75);

            if (Weapon2HRootBone != null)
            {
                Weapon2HRootBone.parent = attachmentsController_WAR.Attach_Weapon2HSheathed;
                Weapon2HRootBone.localPosition = weapon_2H_SheathedLocalOffset;
                Weapon2HRootBone.localRotation = Quaternion.Euler(weapon_2H_SheathedLocalRotation);
            }

            if (Weapon1HRightRootBone != null)
            {
                Weapon1HRightRootBone.parent = attachmentsController_WAR.Attach_Weapon1HRightSheathed;
                Weapon1HRightRootBone.localPosition = weapon_1H_Right_SheathedLocalOffset;
                Weapon1HRightRootBone.localRotation = Quaternion.Euler(weapon_1H_Right_SheathedLocalRotation);
            }

            if (Weapon1HLeftRootBone != null)
            {
                Weapon1HLeftRootBone.parent = attachmentsController_WAR.Attach_Weapon1HLeftSheathed;
                Weapon1HLeftRootBone.localPosition = weapon1HLeftSheathedLocalOffset;
                Weapon1HLeftRootBone.localRotation = Quaternion.Euler(weapon1HLeftSheathedLocalRotation);
            }

            if (ShieldRootBone != null)
            {
                ShieldRootBone.parent = attachmentsController_WAR.Attach_ShieldSheathed;
                ShieldRootBone.localPosition = shieldSheathedLocalOffset;
                ShieldRootBone.localRotation = Quaternion.Euler(shieldSheathedLocalRotation);
            }
        }

        private void SetWeaponPosReady()
        {
            var attachmentsController_WAR = _baseCreature.AttachmentsController as AttachmentsController_WAR;

            Vector3 shieldUnsheathedLocalOffset = new Vector3(0, 0, 0);
            Vector3 shieldUnsheathedLocalRotation = new Vector3(0, 0, 0); // 5 5 8

            if (Weapon2HRootBone != null)
            {
                Weapon2HRootBone.parent = attachmentsController_WAR.Attach_Weapon2HReady;
                Weapon2HRootBone.localPosition = new Vector3(0, 0, 0);
                Weapon2HRootBone.localRotation = Quaternion.identity;
            }

            if (Weapon1HRightRootBone != null)
            {
                Weapon1HRightRootBone.parent = attachmentsController_WAR.Attach_Weapon1HRightReady;
                Weapon1HRightRootBone.localPosition = new Vector3(0, 0, 0);
                Weapon1HRightRootBone.localRotation = Quaternion.identity;
            }

            if (Weapon1HLeftRootBone != null)
            {
                Weapon1HLeftRootBone.parent = attachmentsController_WAR.Attach_Weapon1HLeftReady;
                Weapon1HLeftRootBone.localPosition = new Vector3(0, 0, 0);
                Weapon1HLeftRootBone.localRotation = Quaternion.identity;
            }

            if (ShieldRootBone != null)
            {
                ShieldRootBone.parent = attachmentsController_WAR.Attach_ShieldReady;
                ShieldRootBone.localPosition = shieldUnsheathedLocalOffset;
                ShieldRootBone.localRotation = Quaternion.Euler(shieldUnsheathedLocalRotation);
            }
        }

        private void AutoAttack_WeaponSheathedChanged(bool weaponSheathed)
        {
            // if "Weapon sheathed", then disable "Auto attack"
            if (WeaponSheathed)
            {
                DisableAutoAttackMode();
            }

            _animator.SetBool(ConstantsAnimator.AUTO_ATTACK_BOOL_WEAPON_SHEATHED, weaponSheathed);

            if (weaponSheathed)
                SetWeaponPosSheathed();
            else
                SetWeaponPosReady();
        }

        protected virtual void Update()
        {
            if (AbilityAutoAttack == null)
                return;

            _debugAutoAttackCooldown = AbilityAutoAttack.AbilityCooldown.TimeUntilCooldownFinish;
            
            AbilityAutoAttack.UpdateAbilityTimers();

            if (_baseCreature.ICanSelectTarget.SelectedTarget == null || _baseCreature.ICanSelectTarget.SelectedTarget.IBaseCreature.GetRootObjectTransform() == null)
            {
                WhenNoTarget();
                return;
            }

            var relationWithSelectedTarget = _baseCreature.Faction.GetRelationWith(_baseCreature.ICanSelectTarget.SelectedTarget.IBaseCreature.Faction);

            if (_baseCreature.ICanSelectTarget.SelectedTarget.CanBeAttacked == false ||
                relationWithSelectedTarget > Factions.EFactionRelation.Neutral)
            {
                WhenNoTarget();
            }
            else
            {
                var abilityParameters = AbilityAutoAttack.CreateAbilityParameters(_baseCreature);

                if (CanAttemptToAutoAttack(abilityParameters))
                {
                    StopCurrentAttemptToAttack();

                    TryAutoAttackSelectedTarget();
                }
            }
        }

        private bool CanAttemptToAutoAttack(IAbilityParameters iAbilityParameters)
        {
            var ret =
                CanAutoAttack &&
                AutoAttackModeOn &&
                (_abilitiesController == null || !_abilitiesController.CastAbilityCoroutineWrapper.IsInProgress && !_abilitiesController.IsFinishingCastingAbility) &&
                IsCanStartCast(AbilityAutoAttack, iAbilityParameters) &&
                AttemptingToAutoAttack == false;

            return ret;
        }

        public void TryAutoAttackSelectedTarget()
        {
            var abilityParameters = AbilityAutoAttack.CreateAbilityParameters(_baseCreature);

            _attemptToStartAutoAttackCoroutineWrapper = new CoroutineWrapper(this, AttemptToAutoAttack(_baseCreature.ICanSelectTarget.SelectedTarget, abilityParameters));
            _attemptToStartAutoAttackCoroutineWrapper.StartWrapperCoroutine();
        }

        private void WhenNoTarget()
        {
            DisableAutoAttackMode();
        }

        private void StopCurrentAttemptToAttack()
        {
            if (_attemptToStartAutoAttackCoroutineWrapper.IsInProgress)
            {
                _attemptToStartAutoAttackCoroutineWrapper.StopWrapperCoroutine();

                if (_animator != null)
                {
                    _animator.SetTrigger(ConstantsAnimator.AUTO_ATTACK_TRIGGER_STOP_ATTEMPTING_AUTO_ATTACK);
                }
            }

            AttemptingToAutoAttack = false;
        }

        private bool IsCanStartCast(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (_health.IsAlive == false)
            {
                if (iAbilityParameters.DefaultAbilityParameters.Source is PlayerCreature)
                    GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_WHEN_DEAD);

                return false;
            }

            if (ability.IsAbilityCanStartCast(iAbilityParameters, false) == false)
                return false;

            return true;
        }

        private IEnumerator AttemptToAutoAttack(ITargetable target, IAbilityParameters iAbilityParameters)
        {
            if (IsCanStartCast(AbilityAutoAttack, iAbilityParameters) == false)
                yield break;

            AttemptingToAutoAttack = true;

            if (_animator != null)
            {
                _animator.SetTrigger(ConstantsAnimator.AUTO_ATTACK_TRIGGER_PLAY_AUTO_ATTACK);
            }

            AbilityAutoAttack.SetOnCooldown();

            yield return new WaitForSeconds(AbilityAutoAttackSO.DelayAfterFinishCast);

            FinishAttemptToAutoAttack(target, iAbilityParameters);
        }

        private void FinishAttemptToAutoAttack(ITargetable target, IAbilityParameters iAbilityParameters)
        {
            // если вдруг тригер сам не ресетнулся по каким либо причинам (к примеру сейчас идёт каст какого-то скила и наши анимации авто-атаки игнорируются), 
            // то ресетим его вручную, иначе возможен вариант что он сработает значительно позже (уже после фактического нанесения урона от авто-атаки)
            _animator.ResetTrigger(ConstantsAnimator.AUTO_ATTACK_TRIGGER_PLAY_AUTO_ATTACK);

            AttemptingToAutoAttack = false;
            
            AbilityAutoAttack.CalculateAbilityBehavioursBeforeDelay(AbilityAutoAttack, iAbilityParameters);

            // if (target == null || target.IBaseCreature.GetTransform() == null)
            //     yield break;

            // if (CanAttemptToAutoAttack(target, abilityParameters) == false)
            //     yield break;

            // _autoAttackAbility.ExecuteAbility(abilityParameters);
            AbilityAutoAttack.ExecuteAbility(iAbilityParameters);
        }
    }
}