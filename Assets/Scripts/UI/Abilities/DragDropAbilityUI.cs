using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.SerializableData;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Abilities
{
    [RequireComponent(typeof(AbilityUI))]
    public class DragDropAbilityUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        public AbilityUI AbilityUI { get; private set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            AbilityUI = GetComponent<AbilityUI>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;

            // if (AbilityUI.AbilityUILocation.Location == AbilityUILocation.ELocation.ActionBar)
            // {
            //     AbilityUI.AbilityUILocation.ActionCellInActionBar.AbilityUI = null;
            // }

            AbilityUI.AbilityUILocation.ParentBeforeDrag = this.transform.parent;
            this.transform.SetParent(_canvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Debug.Log("OnEndDrag");

            _canvasGroup.blocksRaycasts = true;

            // изменение позиции должно происходить ДО рейкаста, чтобы рейкасту не мешал сам объект (AbilityIcon и мб ещё что-нибудь)
            _rectTransform.localPosition = new Vector2(0, 0);
            
            this.transform.SetParent(AbilityUI.AbilityUILocation.ParentBeforeDrag);
            AbilityUI.AbilityUILocation.ParentBeforeDrag = null;

            // если предмет перетянут из ActionBar
            if (AbilityUI.AbilityUILocation.Location == AbilityUILocation.ELocation.ActionBar)
            {
                var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
                {
                    position = eventData.position
                };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                // Если абилку "выбросили" на место под которым не найдено GUI.
                if (results.Count == 0)
                {
                    Destroy(this.gameObject);

                    AbilityUI.AbilityUILocation.ActionCellInActionBar.SetAbilityUI(null);
                    AbilityUI.AbilityUILocation.ActionCellInActionBar.UpdateActionCellInActionBar();
                    ActionBarsDataHolder.SerializeActionBarsDataHolder(GameManager.Instance.GUIManager.actionBarsDataHolder);
                }
            }
        }
    }
}