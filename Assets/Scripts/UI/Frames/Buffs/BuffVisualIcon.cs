using System;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects;
using Assets.Scripts.Buffs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class BuffVisualIcon : MonoBehaviour
    {
        [SerializeField] private LayoutElement _imageContainerLayoutElement;
        [SerializeField] private Image _buffImage;
        [SerializeField] private Image _remainingTimeMaskImage;

        [SerializeField] private Transform _textTimerContainer;
        [SerializeField] private TextMeshProUGUI _textTimerText;
        [SerializeField] private Transform _containerStacksCounter;
        [SerializeField] private TextMeshProUGUI _textStacksCounter;

        private CanvasGroup _textTimerCanvasGroup;
        private LayoutElement _textTimerLayoutElement;

        public BuffUI BuffUI { get; private set; }
        public Buff Buff => BuffUI.Buff;

        public float TextTimerHeight
        {
            get
            {
                if (_textTimerLayoutElement == null)
                    _textTimerLayoutElement = _textTimerContainer.GetComponent<LayoutElement>();

                return _textTimerLayoutElement.preferredHeight;
            }
        }

        public bool DisplayRemainingTimeMask = false;

        // public GameObject MainGameObject { get; private set; }

        public bool DisplayTimer = true;

        public void Setup(BuffUI buffUI, bool displayTimer)
        {
            // MainGameObject = this.gameObject;

            _textTimerCanvasGroup = _textTimerContainer.GetComponent<CanvasGroup>();
            _textTimerLayoutElement = _textTimerContainer.GetComponent<LayoutElement>();

            BuffUI = buffUI;
            DisplayTimer = displayTimer;

            _buffImage.sprite = Buff.BaseBuffSO.Icon;

            _textStacksCounter.text = Buff.StacksCount.ToString();
            _containerStacksCounter.gameObject.SetActive(buffUI.Buff.BaseBuffSO.DisplayStacksCountInIcon);

            Buff.StacksCountChanged += BuffOnStacksCountChanged;

            _remainingTimeMaskImage.fillAmount = 0;
            // ImageContainerLayoutElement.preferredWidth = containerGridLayoutGroup.cellSize.x;
            // ImageContainerLayoutElement.preferredHeight = containerGridLayoutGroup.cellSize.y;

            UpdateVisualIcon();
        }

        private void BuffOnStacksCountChanged()
        {
            _textStacksCounter.text = Buff.StacksCount.ToString();
        }

        private void Update()
        {
            UpdateVisualIcon();
        }

        private void UpdateVisualIcon()
        {
            _textTimerContainer.gameObject.SetActive(DisplayTimer);

            if (DisplayTimer)
            {
                HandleTimer();
            }

            if (DisplayRemainingTimeMask)
            {
                // TODO remove "if", do this in some subclass (I suppose?)
                if (Buff.BuffDuration is BuffDurationDefault buffDurationDefault)
                {
                    if (buffDurationDefault.IsPermanent)
                    {
                        _remainingTimeMaskImage.fillAmount = 0;
                    }
                    else
                    {
                        var timeUntilCooldownFinishPercentage = 100 * buffDurationDefault.RemainingDuration / buffDurationDefault.RemainingDurationOnLastUpdateDuration;
                        _remainingTimeMaskImage.fillAmount = 1 - timeUntilCooldownFinishPercentage / 100;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                // if (Buff.BaseBuffSO.BuffDuration == BuffDurationEnum.Permanent)
                // {
                //     _remainingTimeMaskImage.fillAmount = 0;
                // }
                // else
                // {
                //     var timeUntilCooldownFinishPercentage = 100 * Buff.RemainingDuration / Buff.DurationOnLastDurationUpdate;
                //     _remainingTimeMaskImage.fillAmount = 1 - timeUntilCooldownFinishPercentage / 100;
                // }
            }
        }

        private void HandleTimer()
        {
            // TODO remove "if", do this in some subclass (I suppose?)
            if (Buff.BuffDuration is BuffDurationDefault buffDurationDefault)
            {
                if (buffDurationDefault.IsPermanent)
                {
                    _textTimerCanvasGroup.alpha = 0;
                    return;
                }
                
                int buffRemainingDurationInSeconds = Mathf.CeilToInt(buffDurationDefault.RemainingDuration);
                TimeSpan timeLeft = new TimeSpan(0, 0, buffRemainingDurationInSeconds);

                if (timeLeft.Days > 0)
                {
                    _textTimerText.text = $"{timeLeft.Days.ToString()}d";
                }
                else if (timeLeft.Hours > 0)
                {
                    _textTimerText.text = $"{timeLeft.Hours.ToString()}h";
                }
                else if (timeLeft.Minutes > 0)
                {
                    _textTimerText.text = $"{timeLeft.Minutes.ToString()}m";
                }
                else
                {
                    _textTimerText.text = $"{timeLeft.Seconds.ToString()}";
                }

                _textTimerCanvasGroup.alpha = 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _textTimerContainer.gameObject.SetActive(DisplayTimer);
        }
#endif
    }
}