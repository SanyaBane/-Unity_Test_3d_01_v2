using UnityEngine;

namespace Assets.Scripts.UI.Abilities
{
    public class AbilityUILocation
    {
        public enum ELocation
        {
            MenuActions,
            ActionBar
        }

        public ELocation Location { get; private set; } = ELocation.MenuActions;
        public ActionCellInActionBar ActionCellInActionBar { get; private set; }
        public Transform ParentBeforeDrag { get; set; }
        
        public void SetLocationToMenuActions()
        {
            Location = ELocation.MenuActions;
        }

        public void SetLocationToActionCell(ActionCellInActionBar actionCellInActionBar)
        {
            Location = ELocation.ActionBar;
            ActionCellInActionBar = actionCellInActionBar;
        }
    }
}