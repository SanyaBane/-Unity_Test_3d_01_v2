using Assets.Scripts.UI.Abilities;
using System.Linq;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.UI.Frames
{
    public class MenuActions : BaseUIFrame
    {
        public Transform ActionsContainer;

        public GameObject SingleActionContainerPrefab;

        public bool IsOpened => this.gameObject.activeInHierarchy;

        public override void Setup()
        {
            SetAbilitiesInsideMenuActions();
        }

        private void SetAbilitiesInsideMenuActions()
        {
            foreach (var ability in GameManager.Instance.PlayerCreature.AbilitiesController.GetAbilities().OrderBy(x => x.Order))
            {
                var singleActionContainer = Instantiate(SingleActionContainerPrefab, ActionsContainer);
                var menuActionsSingleActionContainerScript = singleActionContainer.GetComponent<MenuActionsSingleActionContainer>();

                menuActionsSingleActionContainerScript.SetAbility(ability);
            }
        }
    }
}
