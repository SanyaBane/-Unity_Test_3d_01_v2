using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public static class OpacityTransitionService
    {
        public static IEnumerator FadeInCoroutine(MonoBehaviour owner, float time, CanvasGroup canvasGroup)
        {
            float currentPassedTime = 0;

            canvasGroup.alpha = 1;

            while (currentPassedTime < time)
            {
                currentPassedTime += Time.deltaTime;

                float currentPassedTimePercentage = currentPassedTime * 100 / time;

                canvasGroup.alpha = 1 - (currentPassedTimePercentage / 100);

                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        public static IEnumerator FadeOutCoroutine(MonoBehaviour owner, float time, CanvasGroup canvasGroup)
        {
            float currentPassedTime = 0;

            canvasGroup.alpha = 0;

            while (currentPassedTime < time)
            {
                currentPassedTime += Time.deltaTime;

                float currentPassedTimePercentage = currentPassedTime * 100 / time;

                canvasGroup.alpha = currentPassedTimePercentage / 100;

                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}
