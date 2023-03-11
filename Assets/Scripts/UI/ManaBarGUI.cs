using System;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ManaSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ManaBarGUI : MonoBehaviour
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
        private float _manaRatio;

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
            newFrameOwner.ManaController.CurrentMPChanged += OwnerCreature_Health_CurrentMPChanged; // subscribe to "new target" MP changes
            OwnerCreature_Health_CurrentMPChanged(newFrameOwner.ManaController); // call manually for first refresh
        }

        protected virtual void OldOwnerUnsubscribe(IBaseCreature oldFrameOwner)
        {
            oldFrameOwner.ManaController.CurrentMPChanged -= OwnerCreature_Health_CurrentMPChanged; // unsubscribe from "previous target" MP changes
        }

        protected void OwnerCreature_Health_CurrentMPChanged(BaseManaController baseManaController)
        {
            SetManaPoints(baseManaController.CurrentMP, BaseManaController.MAX_MANA);
        }

        public void SetManaPoints(int manaPoints, int maxManaPoints)
        {
            _manaRatio = (float) manaPoints / maxManaPoints;
            UpdateMainBar();

            switch (TextFormat)
            {
                case TextFormatEnum.Digits:
                    CurrentValue_TMP.text = manaPoints.ToString();
                    break;
                case TextFormatEnum.Percentage:
                    CurrentValue_TMP.text = $"{Mathf.CeilToInt(_manaRatio * 100)}%";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            UpdateMainBar();
        }
#endif

        private void UpdateMainBar()
        {
            MainBar_Image.fillAmount = _manaRatio;
        }
    }
}