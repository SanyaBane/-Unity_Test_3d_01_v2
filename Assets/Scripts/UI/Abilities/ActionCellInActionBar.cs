using Assets.Scripts.SerializableData;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Abilities
{
    public class ActionCellInActionBar : ActionCell, IDropHandler
    {
        private ActionBar OwnerActionBar { get; set; }

        private void Awake()
        {
            OwnerActionBar = GetComponentInParent<ActionBar>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Debug.Log("OnDrop");

            if (eventData.pointerDrag == null)
                return;

            var draggedAbilityUIGameObject = eventData.pointerDrag;

            // User just drag and drop ability from action bar on same place as it was before.
            if (AbilityUI != null && AbilityUI.gameObject == draggedAbilityUIGameObject)
            {
                draggedAbilityUIGameObject.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                return;
            }

            CloneAbilityUIInsideActionCell(draggedAbilityUIGameObject);

            UpdateActionCellInActionBar();
            ActionBarsDataHolder.SerializeActionBarsDataHolder(GameManager.Instance.GUIManager.actionBarsDataHolder);
        }

        public void CloneAbilityUIInsideActionCell(GameObject draggedAbilityUIGameObject)
        {
            bool isDestroyDraggedAbilityUIGameObject = false;
            var draggedAbilityUI = draggedAbilityUIGameObject.GetComponent<AbilityUI>();
            if (draggedAbilityUI == null)
                return;

            var abilitiesController = GameManager.Instance.PlayerCreature.AbilitiesController;

            // User drag and drop ability from "action bar" to another slot on "action bar"
            if (draggedAbilityUI.AbilityUILocation.Location == AbilityUILocation.ELocation.ActionBar)
            {
                if (AbilityUI != null)
                {
                    // if new slot is already occupied then swap abilities
                    var draggedAbilityUIAbility = draggedAbilityUI.Ability;
                    var draggedGameObjectDragDropScript = draggedAbilityUIGameObject.GetComponent<DragDropAbilityUI>();
                    var draggedActionCellInActionBar = draggedGameObjectDragDropScript.AbilityUI.AbilityUILocation.ActionCellInActionBar;

                    draggedAbilityUI.SetAbility(AbilityUI.Ability, abilitiesController);
                    AbilityUI.SetAbility(draggedAbilityUIAbility, abilitiesController);

                    draggedActionCellInActionBar.UpdateActionCellInActionBar();

                    return;
                }

                // if new slot is empty, move "newAbility" to "currentAbilitySlot"
                isDestroyDraggedAbilityUIGameObject = true;
            }

            if (AbilityUI != null)
            {
                Destroy(AbilityUI.gameObject);
            }

            var clonedAbilityUIGameObject = Instantiate(draggedAbilityUIGameObject, this.transform);
            this.SetAbilityUI(clonedAbilityUIGameObject.GetComponent<AbilityUI>());
            AbilityUI.gameObject.name = draggedAbilityUIGameObject.name;

            AbilityUI.AbilityUILocation.SetLocationToActionCell(this);

            // Maybe ability was dragged from "ActionsWindow", which means "OnEndDrag()" will be not executed on it, so we need to manually reset "blocksRaycasts" to "true"
            AbilityUI.CanvasGroup.blocksRaycasts = true;

            // Manually copy "CurrentAbilityInCell" by reference (Why are we doing this??? Seems like since it's not serializable, it will not be deep-copied by "Instantiate()").
            AbilityUI.SetAbility(draggedAbilityUI.Ability, abilitiesController);

            if (isDestroyDraggedAbilityUIGameObject)
            {
                var draggedGameObjectDragDropScript = draggedAbilityUIGameObject.GetComponent<DragDropAbilityUI>();
                var draggedActionCellInActionBar = draggedGameObjectDragDropScript.AbilityUI.AbilityUILocation.ActionCellInActionBar;

                int newActionCellIndex = draggedActionCellInActionBar.OwnerActionBar.ListActionCellInActionBars.IndexOf(draggedActionCellInActionBar);
                if (newActionCellIndex == -1)
                    Debug.LogError($"{nameof(newActionCellIndex)} == -1");

                var actionCellDataToRemove = GameManager.Instance.GUIManager.actionBarsDataHolder.ListActionBarData
                    .First(x => x.Job == draggedActionCellInActionBar.OwnerActionBar.ActionBarData.Job && x.ActionBarIndex == draggedActionCellInActionBar.OwnerActionBar.Index);

                actionCellDataToRemove.ListActionCellData[newActionCellIndex].ActionId = null;

                Destroy(draggedAbilityUIGameObject);
            }
        }

        public void UpdateActionCellInActionBar()
        {
            EJob currentPlayerJob = GameManager.Instance.PlayerCreature.CurrentJob;
            int actionCellIndex = OwnerActionBar.ListActionCellInActionBars.IndexOf(this);

            var newActionCellData = new ActionCellData()
            {
                ActionId = AbilityUI == null ? null : AbilityUI.Ability.AbilitySO.Id,
                ActionCellIndex = actionCellIndex
            };

            var actionBarData = GameManager.Instance.GUIManager.actionBarsDataHolder.ListActionBarData
                .FirstOrDefault(x => x.Job == currentPlayerJob && x.ActionBarIndex == OwnerActionBar.Index);

            if (actionBarData == null)
            {
                actionBarData = new ActionBarData()
                {
                    Job = currentPlayerJob,
                    ActionBarIndex = OwnerActionBar.Index,
                };

                GameManager.Instance.GUIManager.actionBarsDataHolder.ListActionBarData.Add(actionBarData);
            }

            var existingActionCellData = actionBarData.ListActionCellData
                .FirstOrDefault(x => x.ActionCellIndex == actionCellIndex);

            if (existingActionCellData != null)
            {
                actionBarData.ListActionCellData.Remove(existingActionCellData);
            }

            actionBarData.ListActionCellData.Add(newActionCellData);
            actionBarData.ListActionCellData = actionBarData.ListActionCellData.OrderBy(x => x.ActionCellIndex).ToList();
        }
    }
}