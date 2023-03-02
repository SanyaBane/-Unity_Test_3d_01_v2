using System;
using Assets.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthBarGUI : MonoBehaviour
    {
        public enum TextFormatEnum
        {
            Digits,
            Percentage
        }

        public enum EFrameOwnerType
        {
            SetProgrammatically,
            Player
        }

        [SerializeField] private Image MainBar_Image;
        [SerializeField] private TextMeshProUGUI CurrentValue_TMP;

        public EFrameOwnerType FrameOwnerType = EFrameOwnerType.SetProgrammatically;

        public IBaseCreature CurrentFrameOwner { get; protected set; }

        public bool ShowDigits = true;
        public TextFormatEnum TextFormat = TextFormatEnum.Digits;

        [Header("Debug (readonly)")]
        [SerializeField]
        [Range(0, 1)]
        private float _healthRatio;

        private void Start()
        {
            if (FrameOwnerType == EFrameOwnerType.Player)
            {
                SetNewOwner(GameManager.Instance.PlayerCreature);
            }

            CurrentValue_TMP.gameObject.SetActive(ShowDigits);
        }

        public virtual void SetNewOwner(IBaseCreature newFrameOwner)
        {
            // if target changed for same target as before (is it even possible?), then do nothing
            if (newFrameOwner != null && CurrentFrameOwner == newFrameOwner)
                return;

            if (CurrentFrameOwner != null)
                OldOwnerUnsubscribe(CurrentFrameOwner);

            if (newFrameOwner == null)
            {
                CurrentFrameOwner = null;
            }
            else
            {
                CurrentFrameOwner = newFrameOwner;

                NewOwnerSubscribe(CurrentFrameOwner);
            }
        }

        protected virtual void NewOwnerSubscribe(IBaseCreature newFrameOwner)
        {
            newFrameOwner.Health.CurrentHPChanged += OwnerCreature_Health_CurrentHPChanged; // subscribe to "new target" HP changes
            OwnerCreature_Health_CurrentHPChanged(newFrameOwner.Health); // call manually for first refresh
        }

        protected virtual void OldOwnerUnsubscribe(IBaseCreature oldFrameOwner)
        {
            oldFrameOwner.Health.CurrentHPChanged -= OwnerCreature_Health_CurrentHPChanged; // unsubscribe from "previous target" HP changes
        }

        protected void OwnerCreature_Health_CurrentHPChanged(BaseHealth baseHealth)
        {
            SetHitPoints(baseHealth.CurrentHP, baseHealth.MaxHP);
        }

        public void SetHitPoints(int hitPoints, int maxHitPoints)
        {
            _healthRatio = (float) hitPoints / maxHitPoints;
            UpdateHealthBar();

            switch (TextFormat)
            {
                case TextFormatEnum.Digits:
                    CurrentValue_TMP.text = hitPoints.ToString();
                    break;
                case TextFormatEnum.Percentage:
                    CurrentValue_TMP.text = $"{Mathf.CeilToInt(_healthRatio * 100)}%";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            UpdateHealthBar();
        }
#endif

        private void UpdateHealthBar()
        {
            MainBar_Image.fillAmount = _healthRatio;
        }
    }
}