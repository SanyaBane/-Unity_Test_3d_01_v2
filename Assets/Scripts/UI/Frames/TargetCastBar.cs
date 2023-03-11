using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.UI
{
    public class TargetCastBar : BaseUIFrame
    {
        public static readonly Color BACKGROUND_COLOR_DEFAULT = new Color(1, 1, 1, 0.5f);
        public static readonly Color BACKGROUND_COLOR_INTERRUPTED = new Color(1, 0.25f, 0.25f, 0.5f);
        public const float SIMPLE_CAST_BAR_FADE_OUT = 0.1f;
        public const float SIMPLE_CAST_BAR_FADE_IN_FINISH = 0.8f;
        public const float SIMPLE_CAST_BAR_FADE_IN_INTERRUPT = 0.8f;

        public Transform Background;
        public Transform Filler;
        public TextMeshProUGUI TextMeshProUGUI;

        private CanvasGroup _canvasGroup;

        private Image _backgroundImage;
        private Image _fillerImage;

        private Ability _currentlyCastedAbility;

        private CoroutineWrapper FadeInCoroutineWrapper = new CoroutineWrapper();
        private CoroutineWrapper FadeOutCoroutineWrapper = new CoroutineWrapper();

        private IBaseCreature _currentFrameOwner;

        public override void Setup()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();

            _fillerImage = Filler.GetComponent<Image>();
            _backgroundImage = Background.GetComponent<Image>();
            
            // _canvasGroup.alpha = 0;
        }

        public void UpdateOwnerInfo(IBaseCreature newFrameOwner)
        {
            // if target changed for same target as before (is it even possible?), then do nothing
            if (newFrameOwner != null && _currentFrameOwner == newFrameOwner)
                return;

            if (_currentFrameOwner != null)
                OldOwnerUnsubscribe(_currentFrameOwner);

            if (newFrameOwner == null)
            {
                _currentFrameOwner = null;
            }
            else
            {
                _currentFrameOwner = newFrameOwner;

                NewOwnerSubscribe(_currentFrameOwner);

                if (_currentFrameOwner.AbilitiesController.CastAbilityCoroutineWrapper.IsInProgress)
                {
                    _canvasGroup.alpha = 1;

                    var ability = _currentFrameOwner.AbilitiesController.CastAbilityCoroutineWrapper.Ability;
                    if (ability.IsCastInstant(_currentFrameOwner) == false)
                    {
                        SetupCastingBar(ability);
                        _canvasGroup.alpha = 1;
                    }
                }
                else
                {
                    _canvasGroup.alpha = 0;
                }
            }
        }

        private void NewOwnerSubscribe(IBaseCreature frameOwner)
        {
            frameOwner.AbilitiesController.CastStarted += AbilitiesControllerOnCastStarted;
            frameOwner.AbilitiesController.CastTicked += AbilitiesControllerOnCastTicked;
            frameOwner.AbilitiesController.CastInterrupted += AbilitiesControllerOnCastInterrupted;
            frameOwner.AbilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
        }

        private void AbilitiesControllerOnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(_currentFrameOwner) == false)
            {
                SetupCastingBar(ability);
                FadeOut(SIMPLE_CAST_BAR_FADE_OUT);
            }
        }
        
        private void AbilitiesControllerOnCastTicked(AbilitiesController abilitiesController, Ability ability, float currentlyCastedTime)
        {
            UpdateCastingBar(currentlyCastedTime);
        }
        
        private void AbilitiesControllerOnCastInterrupted(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(_currentFrameOwner) == false)
            {
                DisplayCastInterruption(SIMPLE_CAST_BAR_FADE_IN_INTERRUPT);
            }
        }

        private void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(_currentFrameOwner) == false)
            {
                FadeIn(SIMPLE_CAST_BAR_FADE_IN_FINISH);
            }
        }

        private void OldOwnerUnsubscribe(IBaseCreature frameOwner)
        {
            frameOwner.AbilitiesController.CastStarted -= AbilitiesControllerOnCastStarted;
            frameOwner.AbilitiesController.CastTicked -= AbilitiesControllerOnCastTicked;
            frameOwner.AbilitiesController.CastInterrupted -= AbilitiesControllerOnCastInterrupted;
            frameOwner.AbilitiesController.CastFinished -= AbilitiesControllerOnCastFinished;
        }

        private void SetupCastingBar(Ability ability)
        {
            if (FadeInCoroutineWrapper.IsInProgress)
            {
                FadeInCoroutineWrapper.StopWrapperCoroutine(); // todo фикс многократного нажатия на скил. Тупит или UI или сам скил.
            }

            ShowUIElement();

            _currentlyCastedAbility = ability;

            TextMeshProUGUI.text = ability.AbilitySO.Name;
            _fillerImage.fillAmount = 0;
            _canvasGroup.alpha = 0;

            _backgroundImage.color = BACKGROUND_COLOR_DEFAULT;
        }

        private void UpdateCastingBar(float currentlyCastedTime)
        {
            float currentlyCastedPercentage = currentlyCastedTime * 100 / _currentlyCastedAbility.CastTime;
            _fillerImage.fillAmount = currentlyCastedPercentage / 100;
        }

        private void DisplayCastInterruption(float fadeInTime)
        {
            _backgroundImage.color = BACKGROUND_COLOR_INTERRUPTED;

            this.FadeIn(fadeInTime);
        }

        private void FadeIn(float fadeInTime)
        {
            if (FadeInCoroutineWrapper.IsInProgress)
            {
                FadeInCoroutineWrapper.StopWrapperCoroutine();
            }

            FadeInCoroutineWrapper = new CoroutineWrapper(this, OpacityTransitionService.FadeInCoroutine(this, fadeInTime, _canvasGroup));
            FadeInCoroutineWrapper.StartWrapperCoroutine();
        }

        private void FadeOut(float fadeOutTime)
        {
            if (FadeOutCoroutineWrapper.IsInProgress)
            {
                FadeOutCoroutineWrapper.StopWrapperCoroutine();
            }

            FadeOutCoroutineWrapper = new CoroutineWrapper(this, OpacityTransitionService.FadeOutCoroutine(this, fadeOutTime, _canvasGroup));
            FadeOutCoroutineWrapper.StartWrapperCoroutine();
        }
    }
}