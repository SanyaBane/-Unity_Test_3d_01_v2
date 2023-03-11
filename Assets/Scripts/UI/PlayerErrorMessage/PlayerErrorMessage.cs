using System.Collections;
using Assets.Scripts.Utilities;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.PlayerMessages
{
    public class PlayerErrorMessage : MonoBehaviour
    {
        public const float TimeBeforeFadeOut = 1.0f;
        public const float FadeOutTime = 1.0f;

        private TextMeshProUGUI _textMeshProUGUI;
        private CanvasGroup _canvasGroup;

        private CoroutineWrapper DisplayErrorTextCoroutineWrapper = new CoroutineWrapper();

        public float LastTimeDisplayed { get; private set; } = 0f;

        private void Awake()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;
        }

        public void DisplayErrorMessage(string text)
        {
            if (DisplayErrorTextCoroutineWrapper.IsInProgress)
                DisplayErrorTextCoroutineWrapper.StopWrapperCoroutine();

            DisplayErrorTextCoroutineWrapper = new CoroutineWrapper(this, DisplayErrorMessageCoroutine(text));
            DisplayErrorTextCoroutineWrapper.StartWrapperCoroutine();
        }

        private IEnumerator DisplayErrorMessageCoroutine(string text)
        {
            LastTimeDisplayed = Time.time;

            _textMeshProUGUI.text = text;
            _canvasGroup.alpha = 1;

            yield return new WaitForSeconds(TimeBeforeFadeOut);

            float fadeOutTimeStart = Time.time;
            while (_canvasGroup.alpha > 0)
            {
                float timePassedFromFadeOutStart = Time.time - fadeOutTimeStart;
                float delta = 1 / FadeOutTime * timePassedFromFadeOutStart;
                _canvasGroup.alpha = 1 - delta;

                yield return null;
            }

            yield break;
        }
    }
}