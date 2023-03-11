using System;
using System.Collections;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Buffs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CreatureBuffsUI : LayoutGroup
    {
        public enum EArrangeMode
        {
            Manual,
            AutoWidth
        }

        [SerializeField] private GameObject _buffVisualIconPrefab;

        [SerializeField] private Transform _verticalLayoutRoot;
        [SerializeField] private GameObject _horizontalLayoutRowPrefab;

        private LayoutElement _layoutElement;
        private LayoutElement LayoutElement
        {
            get
            {
                if (_layoutElement == null)
                    _layoutElement = this.GetComponent<LayoutElement>();

                if (_layoutElement == null)
                    Debug.LogError($"{nameof(_layoutElement)} == null.");

                return _layoutElement;
            }
        }

        public EArrangeMode ArrangeMode = EArrangeMode.Manual;
        public Vector2Int MaxRowsColumns = new Vector2Int(2, 2);

        public Vector2 initialCellSize = new Vector2(15, 15);

        private int MaxRows => MaxRowsColumns.x;
        private int MaxColumns
        {
            get
            {
                switch (ArrangeMode)
                {
                    case EArrangeMode.Manual:
                        return MaxRowsColumns.y;
                    case EArrangeMode.AutoWidth:
                        var ret = Mathf.FloorToInt(rectTransform.rect.width / _cellSize.x);
                        return ret;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int MaxIconsDisplay = 16;

        private List<HorizontalLayoutRow> _horizontalLayoutRowList = new List<HorizontalLayoutRow>();

        private CreatureFrame _creatureFrame;

        private BuffsController OwnerBuffsController => _creatureFrame.CurrentFrameOwner.BuffsController;

        private ObservableCollection<BuffUI> _listBuffUI = new ObservableCollection<BuffUI>();

        [SerializeField] private bool _isDisplayTimer = false;
        public bool IsDisplayTimer
        {
            get => _isDisplayTimer;
            set
            {
                _isDisplayTimer = value;
                DisplayTimerChanged?.Invoke(value);
            }
        }
        public event Action<bool> DisplayTimerChanged;

        [Header("Debug (readonly)")]
        [SerializeField] private Vector2 _cellSize = new Vector2(0, 0);

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying)
                return;

            DisplayTimerChanged += OnDisplayTimerChanged;

            for (int i = 0; i < MaxRows; i++)
            {
                var newHorizontalLayoutRow = CreateHorizontalLayoutRowGameObject();
                _horizontalLayoutRowList.Add(newHorizontalLayoutRow);
            }
        }

        private HorizontalLayoutRow CreateHorizontalLayoutRowGameObject()
        {
            var go = Instantiate(_horizontalLayoutRowPrefab, _verticalLayoutRoot);
            var ret = go.GetComponent<HorizontalLayoutRow>();
            return ret;
        }

        private void ReArrangeBuffIcons()
        {
            var orderedListBuffUI = _listBuffUI.ToList();

            int maxColumns = MaxColumns;
            int maxRows = MaxRows;

            // set columns / rows
            for (int i = 0; i < orderedListBuffUI.Count; i++)
            {
                BuffUI buffUI = orderedListBuffUI[i];

                if (i + 1 > maxColumns * maxRows ||
                    i + 1 > MaxIconsDisplay)
                {
                    break;
                }

                int tmpRow = Mathf.CeilToInt((i + 1) / (float) maxColumns);
                buffUI.Row = tmpRow == 0 ? 1 : tmpRow;
                int tmpColumn = (i + 1) % maxColumns;
                buffUI.Column = tmpColumn == 0 ? maxColumns : tmpColumn;

                // Debug.Log($"Row: {buffUI.Row}; Column: {buffUI.Column}.");
            }

            // move buff icons to respective horizontal layout groups
            for (int i = 0; i < orderedListBuffUI.Count; i++)
            {
                BuffUI buffUI = orderedListBuffUI[i];

                if (i + 1 > maxColumns * maxRows ||
                    i + 1 > MaxIconsDisplay)
                {
                    if (buffUI.BuffVisualIcon != null)
                    {
                        Destroy(buffUI.BuffVisualIcon.gameObject);
                        buffUI.BuffVisualIcon = null;
                    }

                    continue;
                }

                var horizontalLayoutRow = _horizontalLayoutRowList[buffUI.Row - 1];

                if (buffUI.BuffVisualIcon == null)
                {
                    var buffVisualIconGameObject = Instantiate(_buffVisualIconPrefab, transform);

                    var buffVisualIconScript = buffVisualIconGameObject.GetComponent<BuffVisualIcon>();
                    buffVisualIconScript.Setup(buffUI, this.IsDisplayTimer);

                    buffUI.BuffVisualIcon = buffVisualIconScript;
                }

                buffUI.BuffVisualIcon.transform.SetParent(horizontalLayoutRow.transform);
                buffUI.BuffVisualIcon.transform.SetAsLastSibling();
            }
        }

        private void RedrawTest()
        {
            float height = _cellSize.y * MaxRows;

            switch (ArrangeMode)
            {
                case EArrangeMode.Manual:
                    float width = _cellSize.x * MaxColumns;
                    LayoutElement.preferredWidth = width;
                    break;
                case EArrangeMode.AutoWidth:
                    LayoutElement.preferredWidth = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            LayoutElement.preferredHeight = height;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (MaxRowsColumns.x < 1)
                MaxRowsColumns.x = 1;

            if (MaxRowsColumns.y < 1)
                MaxRowsColumns.y = 1;

            OnDisplayTimerChanged(IsDisplayTimer);
            RedrawTest();  
        }
#endif

        private void OnDisplayTimerChanged(bool isDisplayTimer)
        {
            try
            {
                var buffVisualIconScript = _buffVisualIconPrefab.GetComponent<BuffVisualIcon>(); // possible "MissingReferenceException" when access prefab GameObject
                float textTimerHeight = buffVisualIconScript.TextTimerHeight;

                var hmtCellSize = initialCellSize;
                if (isDisplayTimer)
                    _cellSize = new Vector2(hmtCellSize.x, hmtCellSize.y + textTimerHeight);
                else
                    _cellSize = new Vector2(hmtCellSize.x, hmtCellSize.y);

                RedrawTest();
            }
            catch (MissingReferenceException ex)
            {
                Debug.LogError($"OnDisplayTimerChanged: {ex.Message}");
            }
        }

        public void SetupCreatureFrame(CreatureFrame creatureFrame)
        {
            _creatureFrame = creatureFrame;
        }

        public void NewOwnerSubscribe(IBaseCreature frameOwner)
        {
            UpdateOwnerInfo();

            frameOwner.BuffsController.BuffAdded += BuffsControllerOnBuffAdded;
            frameOwner.BuffsController.BuffRemoved += BuffsControllerOnBuffRemoved;
        }

        public void OldOwnerUnsubscribe(IBaseCreature frameOwner)
        {
            frameOwner.BuffsController.BuffAdded -= BuffsControllerOnBuffAdded;
            frameOwner.BuffsController.BuffRemoved -= BuffsControllerOnBuffRemoved;
        }

        private void UpdateOwnerInfo()
        {
            // clear all UIIcons
            for (int i = _listBuffUI.Count - 1; i >= 0; i--)
            {
                var buffUI = _listBuffUI[i];

                if (buffUI.BuffVisualIcon != null)
                {
                    Destroy(buffUI.BuffVisualIcon.gameObject);
                    buffUI.BuffVisualIcon = null;
                }

                _listBuffUI.RemoveAt(i);
            }

            // create icons from currently existed buffs
            foreach (var buff in _creatureFrame.CurrentFrameOwner.BuffsController.GetAllBuffs())
            {
                var buffUI = new BuffUI(buff);
                _listBuffUI.Add(buffUI);
            }
            
            StartCoroutine(UpdateOnNextFrame());
        }

        private IEnumerator UpdateOnNextFrame()
        {
            // К сожалению, если мы попытаемся сделать "RedrawTest" в режиме "AutoWidth" незамедлительно, то есть вероятность что в этот момент
            // "rectTransform" у текущего LayoutGroup имеет ширину "0", в связи с чем MaxColumns также будет "0", так что лучше
            // выполнить это на следующем фрейме, когда ширина LayoutGroup уже будет корректно вычислена.
            yield return new WaitForEndOfFrame();

            RedrawTest();
            ReArrangeBuffIcons();
        }

        private void BuffsControllerOnBuffAdded(Buff buff)
        {
            var buffUI = new BuffUI(buff);
            _listBuffUI.Add(buffUI);

            ReArrangeBuffIcons();
        }

        private void BuffsControllerOnBuffRemoved(Buff buff)
        {
            RemoveBuffIcon(buff);

            ReArrangeBuffIcons();
        }

        private void RemoveBuffIcon(Buff buff)
        {
            var buffUI = _listBuffUI.FirstOrDefault(x => x.Buff == buff);
            if (buffUI == null)
            {
                Debug.LogError($"{nameof(buffUI)} == null.");
                return;
            }

            if (buffUI.BuffVisualIcon != null)
            {
                Destroy(buffUI.BuffVisualIcon.gameObject);
                buffUI.BuffVisualIcon = null;
            }

            _listBuffUI.Remove(buffUI);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            // Debug.Log("CalculateLayoutInputHorizontal()");
        }

        public override void CalculateLayoutInputVertical()
        {
            // Debug.Log("CalculateLayoutInputVertical()");
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        // protected override void OnRectTransformDimensionsChange()
        // {
        //     base.OnRectTransformDimensionsChange();
        // }
    }
}