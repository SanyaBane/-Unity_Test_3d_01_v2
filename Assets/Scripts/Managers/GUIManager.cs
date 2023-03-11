using Assets.Scripts.SerializableData;
using Assets.Scripts.UI.Abilities;
using Assets.Scripts.UI.Frames;
using Assets.Scripts.UI.PlayerMessages;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GUIManager : MonoBehaviour
    {
        public Transform TestText_Container;

        [SerializeField] private PlayerFrame _playerFrame;
        [SerializeField] private TargetFrame _targetFrame;
        [SerializeField] private PlayerCastBar _playerCastBar;

        public ActionBarsContainer ActionBarsContainer;

        public MenuActions ActionsWindow;
        public CombatTargetsUI CombatTargetsUI;
        public PlayerErrorMessageContainer PlayerErrorMessageContainer;
        public PartyFrame PartyFrame;

        public GameObject AbilityUIPrefab;

        public ActionBarsDataHolder actionBarsDataHolder { get; private set; }

        private void Awake()
        {
            if (_playerFrame == null)
                Debug.LogError($"{nameof(_playerFrame)} == null");

            if (_targetFrame == null)
                Debug.LogError($"{nameof(_targetFrame)} == null");

            if (_playerCastBar == null)
                Debug.LogError($"{nameof(_playerCastBar)} == null");

            if (ActionBarsContainer == null)
                Debug.LogError($"{nameof(ActionBarsContainer)} == null");

            if (ActionsWindow == null)
                Debug.LogError($"{nameof(ActionsWindow)} == null");
        }

        public void TestResetActionBarsData()
        {
            // Debug.Log("TestResetActionBarsData");

            PlayerPrefs.DeleteKey(Constants.PLAYER_PREFS_KEY_LIST_ACTION_BARS);

            actionBarsDataHolder = ActionBarsDataHolder.DeserializeActionBarsDataHolder();

            foreach (var actionBar in ActionBarsContainer.ActionBars)
            {
                actionBar.Setup();
            }
        }

        private void Start()
        {
            actionBarsDataHolder = ActionBarsDataHolder.DeserializeActionBarsDataHolder();

            foreach (var actionBar in ActionBarsContainer.ActionBars)
            {
                actionBar.Setup();
            }

            _playerCastBar.Setup();

            // _playerFrame.Init();
            _playerFrame.Setup();
            var playerCreature = GameManager.Instance.PlayerCreature;
            _playerFrame.SetNewOwner(playerCreature);

            _targetFrame.Setup();
            ActionsWindow.Setup();
            CombatTargetsUI.Setup();

            _playerCastBar.HideUIElement();
            _targetFrame.HideUIElement();
            ActionsWindow.HideUIElement();
        }

        private void Update()
        {
            if (GameManager.Instance.InputController_WoW.IsMenuActionsPressed)
            {
                if (ActionsWindow.IsOpened)
                    ActionsWindow.HideUIElement();
                else
                    ActionsWindow.ShowUIElement();
            }

            if (GameManager.Instance.InputController_WoW.CommandCallEscapeMenu)
            {
                // TODO implement stack of UI windows - top of stack should hide when "CommandCallEscapeMenu == true"
                if (ActionsWindow.IsOpened)
                    ActionsWindow.HideUIElement();
            }
        }

        public void UpdateUIVariables(int uiPos, string str)
        {
            if (TestText_Container == null)
                return;

            Transform testText_Container_Child = TestText_Container.GetChild(uiPos);

            var text = testText_Container_Child.GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                Debug.LogError($"{nameof(text)} == null | uiPos = " + uiPos);
                return;
            }

            text.text = str;
        }
    }
}