using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Frames
{
    public class PlayerCastBar : BaseUIFrame
    {
        public static readonly Color BACKGROUND_COLOR_DEFAULT = new Color(1, 1, 1, 0.5f);
        public static readonly Color BACKGROUND_COLOR_INTERRUPTED = new Color(1, 0.25f, 0.25f, 0.5f);
        public const float SIMPLE_CAST_BAR_FADE_OUT = 0.1f;
        public const float SIMPLE_CAST_BAR_FADE_IN_FINISH = 0.8f;
        public const float SIMPLE_CAST_BAR_FADE_IN_INTERRUPT = 0.8f;

        public Transform Background;
        public Transform Filler;
        public TextMeshProUGUI TextMeshProUGUI;
        public Image AbilityIconImage;

        private CanvasGroup _canvasGroup;

        private Image _backgroundImage;
        private Image _fillerImage;

        private Ability _currentlyCastedAbility;

        private CoroutineWrapper FadeInCoroutineWrapper = new CoroutineWrapper();
        private CoroutineWrapper FadeOutCoroutineWrapper = new CoroutineWrapper();

        public IBaseCreature FrameOwner { get; protected set; }

        public override void Setup()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();

            _fillerImage = Filler.GetComponent<Image>();
            _backgroundImage = Background.GetComponent<Image>();

            var playerCreatureInfoContainer = GameManager.Instance.PlayerGameObject.GetComponent<CreatureInfoContainer>();
            var playerFrameOwner = playerCreatureInfoContainer.BaseCreature;

            UpdateOwnerInfo(playerFrameOwner);
        }

        protected void UpdateOwnerInfo(IBaseCreature newFrameOwner)
        {
            // if target changed for same target as before (is it even possible?), then do nothing
            if (newFrameOwner != null && FrameOwner == newFrameOwner)
                return;

            if (FrameOwner != null)
                OldOwnerUnsubscribe(FrameOwner);

            if (newFrameOwner == null)
            {
                FrameOwner = null;
            }
            else
            {
                FrameOwner = newFrameOwner;

                NewOwnerSubscribe(FrameOwner);
            }
        }

        private void NewOwnerSubscribe(IBaseCreature frameOwner)
        {
            frameOwner.AbilitiesController.CastStarted += AbilitiesControllerOnCastStarted;
            frameOwner.AbilitiesController.CastTicked += AbilitiesControllerOnCastTicked;
            frameOwner.AbilitiesController.CastInterrupted += AbilitiesControllerOnCastInterrupted;
            frameOwner.AbilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
        }

        private void AbilitiesControllerOnCastTicked(AbilitiesController abilitiesController, Ability ability, float currentlyCastedTime)
        {
            UpdateCastingBar(currentlyCastedTime);
        }

        private void AbilitiesControllerOnCastStarted(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(FrameOwner) == false)
            {
                SetupCastingBar(ability);
                FadeOut(SIMPLE_CAST_BAR_FADE_OUT);
            }
        }

        private void AbilitiesControllerOnCastInterrupted(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(FrameOwner) == false)
            {
                InterruptCast(SIMPLE_CAST_BAR_FADE_IN_INTERRUPT);
            }
        }

        private void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            if (ability.IsCastInstant(FrameOwner) == false)
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

        public void SetupCastingBar(Ability ability)
        {
            if (FadeInCoroutineWrapper.IsInProgress)
            {
                FadeInCoroutineWrapper.StopWrapperCoroutine(); // todo фикс многократного нажатия на скил. Тупит или UI или сам скил.
            }

            ShowUIElement();

            _currentlyCastedAbility = ability;

            AbilityIconImage.sprite = ability.AbilitySO.Icon;
            TextMeshProUGUI.text = ability.AbilitySO.Name;
            _fillerImage.fillAmount = 0;
            _canvasGroup.alpha = 0;

            _backgroundImage.color = BACKGROUND_COLOR_DEFAULT;
        }

        public void UpdateCastingBar(float currentlyCastedTime)
        {
            float curentlyCastedPercentage = currentlyCastedTime * 100 / _currentlyCastedAbility.CastTime;
            _fillerImage.fillAmount = curentlyCastedPercentage / 100;
        }

        public void InterruptCast(float fadeInTime)
        {
            _backgroundImage.color = BACKGROUND_COLOR_INTERRUPTED;

            this.FadeIn(fadeInTime);
        }

        public void FadeIn(float fadeInTime)
        {
            if (FadeInCoroutineWrapper.IsInProgress)
            {
                FadeInCoroutineWrapper.StopWrapperCoroutine();
            }
            if (FadeOutCoroutineWrapper.IsInProgress)
            {
                FadeOutCoroutineWrapper.StopWrapperCoroutine();
            }

            FadeInCoroutineWrapper = new CoroutineWrapper(this, OpacityTransitionService.FadeInCoroutine(this, fadeInTime, _canvasGroup));
            FadeInCoroutineWrapper.StartWrapperCoroutine();
        }

        public void FadeOut(float fadeOutTime)
        {
            if (FadeInCoroutineWrapper.IsInProgress)
            {
                FadeInCoroutineWrapper.StopWrapperCoroutine();
            }
            if (FadeOutCoroutineWrapper.IsInProgress)
            {
                FadeOutCoroutineWrapper.StopWrapperCoroutine();
            }

            FadeOutCoroutineWrapper = new CoroutineWrapper(this, OpacityTransitionService.FadeOutCoroutine(this, fadeOutTime, _canvasGroup));
            FadeOutCoroutineWrapper.StartWrapperCoroutine();
        }
    }
}