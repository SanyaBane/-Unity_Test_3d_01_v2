using Assets.Scripts.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Abilities
{
    public class AbilityUI : MonoBehaviour
    {
        public Image AbilityImage;
        public Image AvailabilityMaskImage;
        public Image CooldownMaskImage;
        public Image ComboVFXImage;
        public Transform ContainerManaCost;
        public TextMeshProUGUI ManaCostTMP;

        public CanvasGroup CanvasGroup { get; private set; }

        public readonly AbilityUILocation AbilityUILocation = new AbilityUILocation();

        private AbilitiesController _abilitiesController;
        public Ability Ability { get; private set; }

        private CoroutineWrapper _animateComboCoroutineWrapper = new CoroutineWrapper();

        #region Animate Combo Properties

        private float comboActionFadeMinAlphaValue = 0.0f;
        private float comboActionFadeMaxAlphaValue = 0.5f;
        private float comboActionFadeSpeed = 1f;

        #endregion

        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            CooldownMaskImage.fillAmount = 0;
        }

        private void Update()
        {
            if (Ability == null)
                return;

            #region Combo

            if (Ability.TimeUntilComboActionAvailable > 0 && !_animateComboCoroutineWrapper.IsInProgress)
            {
                AnimateComboEffect();
            }
            else if (Ability.TimeUntilComboActionAvailable == 0 && _animateComboCoroutineWrapper.IsInProgress)
            {
                _animateComboCoroutineWrapper.StopWrapperCoroutine();
            }

            #endregion

            UpdateAvailability();

            UpdateManaCost();
        }

        public void SetAbility(Ability ability, AbilitiesController abilitiesController)
        {
            _abilitiesController = abilitiesController;
            Ability = ability;

            if (Ability == null)
                return;

            if (Ability.AbilitySO.Icon == null)
            {
                var abilityNoImagePlaceholder = Resources.Load<Sprite>(ConstantsResources.ICON_ABILITY_NO_IMAGE);
                if (abilityNoImagePlaceholder == null)
                    Debug.LogError(Constants.MESSAGE_RESOURCE_NOT_FOUNDED);

                AbilityImage.sprite = abilityNoImagePlaceholder;
            }
            else
            {
                AbilityImage.sprite = ability.AbilitySO.Icon;
            }
        }

        private void UpdateAvailability()
        {
            var abilityParameters = Ability.CreateAbilityParameters(_abilitiesController.IBaseCreature);

            if (_abilitiesController.IsCastingAbility ||
                Ability.IsAvailable(abilityParameters, false) == false)
            {
                AvailabilityMaskImage.gameObject.SetActive(true);
            }
            else
            {
                AvailabilityMaskImage.gameObject.SetActive(false);
            }

            if (Ability.AbilityCooldown.TimeUntilCooldownFinish > 0 ||
                (Ability.AbilityCooldown.GetAffectsGlobalCooldown && _abilitiesController.TimeUntilGlobalCooldownFinish > 0))
            {
                DrawAbilityCooldown();
                return;
            }

            DrawAbilityAvailable();
        }

        private void UpdateManaCost()
        {
            bool showManaCostLabel = Ability.ManaCost != 0;

            ContainerManaCost.gameObject.SetActive(showManaCostLabel);

            if (showManaCostLabel)
            {
                ManaCostTMP.text = Ability.ManaCost.ToString();
            }
        }

        private void DrawAbilityCooldown()
        {
            float timeUntilCooldownFinishPercentage = 0;
            if (Ability.AbilityCooldown.GetAbilityCooldownOnLastCast != 0)
            {
                timeUntilCooldownFinishPercentage = 100 * Ability.AbilityCooldown.TimeUntilCooldownFinish / Ability.AbilityCooldown.GetAbilityCooldownOnLastCast;
            }

            float timeUntilGlobalCooldownFinishPercentage = 0;
            if (_abilitiesController.BaseGlobalCooldown != 0)
            {
                timeUntilGlobalCooldownFinishPercentage = 100 * _abilitiesController.TimeUntilGlobalCooldownFinish / _abilitiesController.BaseGlobalCooldown;
            }

            float greatestCooldown = Mathf.Max(timeUntilCooldownFinishPercentage, timeUntilGlobalCooldownFinishPercentage);

            CooldownMaskImage.fillAmount = greatestCooldown / 100;
        }

        private void DrawAbilityAvailable()
        {
            if (CooldownMaskImage.fillAmount != 0)
                CooldownMaskImage.fillAmount = 0;
        }

        public void TryExecuteAbilityInsideActionCell()
        {
            if (Ability == null)
                return;

            _abilitiesController.TryStartCast(Ability);
        }

        private void AnimateComboEffect()
        {
            _animateComboCoroutineWrapper = new CoroutineWrapper(this, AnimateCombo());
            _animateComboCoroutineWrapper.FinishAction = new Action(() => { ComboVFXImage.color = new Color(1, 1, 1, comboActionFadeMinAlphaValue); });
            _animateComboCoroutineWrapper.StartWrapperCoroutine();
        }

        private IEnumerator AnimateCombo()
        {
            float targetAlphaValue = comboActionFadeMaxAlphaValue;
            bool addingToAlpha = false;

            while (true)
            {
                if (Ability.TimeUntilComboActionAvailable == 0)
                    break;

                if (addingToAlpha)
                    targetAlphaValue += comboActionFadeSpeed * Time.deltaTime;
                else
                    targetAlphaValue -= comboActionFadeSpeed * Time.deltaTime;

                if (targetAlphaValue >= comboActionFadeMaxAlphaValue)
                {
                    targetAlphaValue = comboActionFadeMaxAlphaValue;
                    addingToAlpha = false;
                }
                else if (targetAlphaValue <= comboActionFadeMinAlphaValue)
                {
                    targetAlphaValue = comboActionFadeMinAlphaValue;
                    addingToAlpha = true;
                }

                ComboVFXImage.color = new Color(1, 1, 1, targetAlphaValue);
                yield return null;
            }
        }
    }
}