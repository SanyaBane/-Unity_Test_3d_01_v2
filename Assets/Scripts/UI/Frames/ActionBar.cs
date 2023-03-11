using Assets.Scripts.UI.Abilities;
using Assets.Scripts.SerializableData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [DebuggerDisplay("Index: {Index};")]
    public class ActionBar : BaseUIFrame
    {
        [SerializeField] private int _Index;
        public int Index => _Index;

        [SerializeField] private List<ActionCellInActionBar> _ListActionCellInActionBars;

        public List<ActionCellInActionBar> ListActionCellInActionBars => _ListActionCellInActionBars;

        public Dictionary<List<KeyCode>, int> KeysMapping; // controlled by "InputController"

        public ActionBarData ActionBarData { get; private set; }

        public override void Setup()
        {
            EJob currentPlayerJob = GameManager.Instance.PlayerCreature.CurrentJob;

            ActionBarData = GameManager.Instance.GUIManager.actionBarsDataHolder.ListActionBarData
                .FirstOrDefault(x => x.Job == currentPlayerJob && x.ActionBarIndex == Index);

            if (ActionBarData == null)
            {
                ActionBarData = new ActionBarData()
                {
                    ActionBarIndex = Index,
                    Job = currentPlayerJob,
                };
            }

            RemoveActionsFromActionBar();
            FillActionBarFromActionBarData();
        }

        private void RemoveActionsFromActionBar()
        {
            // Пройтись по всем ActionCell и убрать из них абилки
            for (int i = 0; i < ListActionCellInActionBars.Count; i++)
            {
                var actionCellInActionBar = ListActionCellInActionBars[i];
                var abilityUI = actionCellInActionBar.AbilityUI;

                if (abilityUI != null)
                {
                    Destroy(abilityUI.gameObject);
                    //ListActionCellInActionBars[i].AbilityUI = null; // we don't need to assign "null", since by destroying gameobject reference will become "null" automatically, right?
                }
            }
        }

        private void FillActionBarFromActionBarData()
        {
            var abilitiesController = GameManager.Instance.PlayerCreature.AbilitiesController;

            foreach (var actionCellData in ActionBarData.ListActionCellData)
            {
                var ability = GameManager.Instance
                    .PlayerCreature.AbilitiesController
                    .GetAbilities()
                    .FirstOrDefault(x => String.IsNullOrEmpty(x.AbilitySO.Id) == false && x.AbilitySO.Id == actionCellData.ActionId);

                if (ability == null)
                    continue;

                var createdAbilityUIGameObject = Instantiate(GameManager.Instance.GUIManager.AbilityUIPrefab);
                var abilityUIScript = createdAbilityUIGameObject.GetComponent<AbilityUI>();
                abilityUIScript.SetAbility(ability, abilitiesController);

                ListActionCellInActionBars[actionCellData.ActionCellIndex].CloneAbilityUIInsideActionCell(createdAbilityUIGameObject);

                Destroy(createdAbilityUIGameObject);
            }
        }
    }
}