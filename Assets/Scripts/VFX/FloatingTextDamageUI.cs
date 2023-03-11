using System.Collections.Generic;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class FloatingTextDamageUI : MonoBehaviour
    {
        private enum ELifetimePhase
        {
            First,
            Second
        }

        public enum EAnimationMode
        {
            None,
            MoveUp,
            MoveDown
        }

        private Camera _camera;
        private RectTransform _floatingTextContainerRectTransform;
        private RectTransform _floatingTextRectTransform;
        private CanvasGroup _canvasGroup;

        private Transform _target;
        private Vector3 _spawnPos;

        private ELifetimePhase _phase;

        public float TimeActive { get; private set; }

        [SerializeField] private TextMeshProUGUI FloatingText;

        public float FadeOutTime = 0.3f;
        public float TimeUntilReset = 2f;
        public float MoveSpeed = 50;
        public Vector3 LocalOffset;

        [Range(0, 1)]
        public float StartFadeValue = 0.5f;

        private EAnimationMode AnimationMode;

        public void Setup(Transform target, string text, Color color, EAnimationMode animationMode)
        {
            _camera = Camera.main;
            _floatingTextContainerRectTransform = this.GetComponent<RectTransform>();
            _canvasGroup = this.GetComponent<CanvasGroup>();
            _floatingTextRectTransform = FloatingText.GetComponent<RectTransform>();

            FloatingText.text = text;
            FloatingText.color = color;

            _target = target;
            _spawnPos = _target.position;

            AnimationMode = animationMode;

            TimeActive = 0;

            OffsetNeighbourFloatingTextDamageUIs();

            // немного рандомизируем точку появления по оси X
            LocalOffset = Vector3.zero + new Vector3(Random.Range(0, _floatingTextRectTransform.rect.width / 8), 0, 0);

            _phase = ELifetimePhase.First;

            switch (_phase)
            {
                case ELifetimePhase.First:
                    _canvasGroup.alpha = 0;
                    break;

                case ELifetimePhase.Second:
                    _canvasGroup.alpha = 1;
                    break;
            }

            this.gameObject.SetActive(true);
        }

        private void OffsetNeighbourFloatingTextDamageUIs()
        {
            // TODO Сейчас реализовано только для оси Y. Надо бы ещё X.
            // Тогда надо будет использовать другой тип для overlapValue: "float" заменить "Vector2".

            var floatingTextPoolActiveObjects = GameManager.Instance.FloatingTextDamagePool.GetPoolActiveObjects();
            var activeFloatingTextScripts = new List<FloatingTextDamageUI>();
            float overlapValue = 0;
            foreach (var go in floatingTextPoolActiveObjects)
            {
                var floatingTextDamageUI = go.GetComponent<FloatingTextDamageUI>();
                if (floatingTextDamageUI._target != this._target)
                    continue;

                activeFloatingTextScripts.Add(floatingTextDamageUI);

                if (Mathf.Abs(floatingTextDamageUI.LocalOffset.y) < _floatingTextRectTransform.rect.height)
                {
                    overlapValue = _floatingTextRectTransform.rect.height - Mathf.Abs(floatingTextDamageUI.LocalOffset.y);
                    break;
                }
            }

            if (overlapValue > 0)
            {
                foreach (var floatingTextDamageUI in activeFloatingTextScripts)
                {
                    floatingTextDamageUI.LocalOffset += new Vector3(0, overlapValue, 0);
                }
            }
        }

        private void LateUpdate()
        {
            _floatingTextContainerRectTransform.position = _camera.WorldToScreenPoint(_spawnPos);

            LocalOffset += new Vector3(0, MoveSpeed, 0) * Time.deltaTime;

            switch (AnimationMode)
            {
                case EAnimationMode.MoveUp:
                    _floatingTextRectTransform.localPosition = LocalOffset;
                    break;

                case EAnimationMode.MoveDown:
                    _floatingTextRectTransform.localPosition = LocalOffset * -1;
                    break;

                //case EAnimationMode.None:
                //    throw new System.NotImplementedException();
            }

            switch (_phase)
            {
                case ELifetimePhase.First:
                    if (_canvasGroup.alpha < 1)
                    {
                        _canvasGroup.alpha += Mathf.Lerp(0, 1, TimeActive / FadeOutTime);

                        if (_canvasGroup.alpha > 1)
                            _canvasGroup.alpha = 1;
                    }

                    if (_canvasGroup.alpha == 1)
                    {
                        _phase = ELifetimePhase.Second;
                    }

                    break;

                case ELifetimePhase.Second:
                    if (TimeActive >= TimeUntilReset * StartFadeValue)
                        _canvasGroup.alpha = Mathf.InverseLerp(TimeUntilReset, TimeUntilReset * StartFadeValue, TimeActive);
                    else
                        _canvasGroup.alpha = 1;

                    break;
            }


            TimeActive += Time.deltaTime;
            if (TimeActive >= TimeUntilReset)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}