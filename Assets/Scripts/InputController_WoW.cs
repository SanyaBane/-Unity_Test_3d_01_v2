using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InputController_WoW : MonoBehaviour
    {
        public float Vertical;
        public float Horizontal;
        public float Strafe;
        public float MouseScrollWheel;

        public Vector2 MouseInput;

        public bool IsWeaponSheathPressed;
        public bool IsEscapeButtonPressed;
        public bool IsMenuActionsPressed;
        public bool IsNumpadSlashPressed;
        public bool IsPressAutoMoveSwitchPressed;

        public bool IsSprintButtonHolded;

        public bool CommandCancelCast;
        public bool CommandUnselectTarget;
        public bool CommandCallEscapeMenu;

        #region Mouse Left Button

        public bool IsMouseLeftButtonDown;
        public bool IsMouseLeftButtonUp;
        public bool IsMouseLeftButtonHolded;
        public float HowLongMouseLeftButtonHolded { get; private set; } = 0.0f;
        public bool MouseLeftButtonStartHoldOnGUI;

        #endregion

        #region Mouse Right Button

        public bool IsMouseRightButtonDown;
        public bool IsMouseRightButtonUp;
        public bool IsMouseRightButtonHolded;
        public float HowLongMouseRightButtonHolded { get; private set; } = 0.0f;
        public bool MouseRightButtonStartHoldOnGUI;

        #endregion

        public bool SelectPlayerCharacterButtonDown;

        public float TargetSelectionClickAllowedDelay = 0.3f;

        private void Start()
        {
            var actionBar1 = GameManager.Instance.GUIManager.ActionBarsContainer.ActionBars.First(x => x.Index == 1);
            var actionBar2 = GameManager.Instance.GUIManager.ActionBarsContainer.ActionBars.First(x => x.Index == 2);
            var actionBar3 = GameManager.Instance.GUIManager.ActionBarsContainer.ActionBars.First(x => x.Index == 3);

            actionBar1.KeysMapping = new Dictionary<List<KeyCode>, int>()
            {
                {new List<KeyCode> {KeyCode.Alpha1}, 0},
                {new List<KeyCode> {KeyCode.Alpha2}, 1},
                {new List<KeyCode> {KeyCode.Alpha3}, 2},
                {new List<KeyCode> {KeyCode.Alpha4}, 3},
                {new List<KeyCode> {KeyCode.Alpha5}, 4},
                {new List<KeyCode> {KeyCode.Alpha6}, 5},
                {new List<KeyCode> {KeyCode.Alpha7}, 6},
                {new List<KeyCode> {KeyCode.Alpha8}, 7},
                {new List<KeyCode> {KeyCode.Alpha9}, 8},
                {new List<KeyCode> {KeyCode.Alpha0}, 9},
                {new List<KeyCode> {KeyCode.Minus}, 10},
                {new List<KeyCode> {KeyCode.Equals}, 11},
            };

            actionBar2.KeysMapping = new Dictionary<List<KeyCode>, int>()
            {
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha1}, 0},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha2}, 1},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha3}, 2},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha4}, 3},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha5}, 4},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha6}, 5},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha7}, 6},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha8}, 7},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha9}, 8},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Alpha0}, 9},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Minus}, 10},
                {new List<KeyCode> {KeyCode.LeftShift, KeyCode.Equals}, 11},
            };

            actionBar3.KeysMapping = new Dictionary<List<KeyCode>, int>()
            {
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha1}, 0},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha2}, 1},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha3}, 2},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha4}, 3},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha5}, 4},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha6}, 5},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha7}, 6},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha8}, 7},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha9}, 8},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Alpha0}, 9},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Minus}, 10},
                {new List<KeyCode> {KeyCode.LeftControl, KeyCode.Equals}, 11},
            };
        }

        private readonly List<KeyCode> HoldKeys = new List<KeyCode>()
        {
            KeyCode.LeftShift, KeyCode.RightShift,
            KeyCode.LeftControl, KeyCode.RightControl,
        };

        private void Update()
        {
            GetInput();

            // if ( //Input.GetKey(KeyCode.LeftShift) && 
            //     Input.GetKeyDown(KeyCode.Alpha2))
            // {
            // }

            foreach (var actionBar in GameManager.Instance.GUIManager.ActionBarsContainer.ActionBars)
            {
                foreach (var element in actionBar.KeysMapping)
                {
                    var keyCodesList = element.Key;
                    var index = element.Value;

                    bool isModKeyHold = true;
                    foreach (var holdKey in HoldKeys)
                    {
                        if (keyCodesList.Contains(holdKey))
                        {
                            if (!Input.GetKey(holdKey))
                            {
                                isModKeyHold = false;
                            }
                        }
                        else
                        {
                            if (Input.GetKey(holdKey))
                            {
                                isModKeyHold = false;
                            }
                        }
                    }

                    if (!isModKeyHold)
                        continue;

                    bool isKeyPressed = true;
                    foreach (var keyCode in keyCodesList)
                    {
                        if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift ||
                            keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
                        {
                            continue;
                        }

                        isKeyPressed = Input.GetKeyDown(keyCode);
                    }

                    if (isKeyPressed == false)
                        continue;

                    var actionCellInActionBar = actionBar.ListActionCellInActionBars[index];
                    var abilityUI = actionCellInActionBar.AbilityUI;
                    if (abilityUI != null)
                    {
                        abilityUI.TryExecuteAbilityInsideActionCell();
                    }
                }
            }

            if (IsEscapeButtonPressed)
            {
                if (GameManager.Instance.PlayerCreature.AbilitiesController.CastAbilityCoroutineWrapper.IsInProgress)
                {
                    CommandCancelCast = true;
                }
                else if (GameManager.Instance.PlayerCreature.PlayerTargetHandler.SelectedTarget != null)
                {
                    CommandUnselectTarget = true;
                }
                else
                {
                    CommandCallEscapeMenu = true;
                }
            }
        }

        private void GetInput()
        {
            //Vertical = Input.GetAxis("Vertical");
            Vertical = Input.GetAxisRaw("Vertical");
            Horizontal = Input.GetAxisRaw("Horizontal");
            Strafe = Input.GetAxisRaw("Strafe");
            MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

            MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            IsWeaponSheathPressed = Input.GetKeyDown(KeyCode.Z);

            IsEscapeButtonPressed = Input.GetButtonDown("Cancel");
            CommandCancelCast = false;
            CommandUnselectTarget = false;
            CommandCallEscapeMenu = false;

            IsNumpadSlashPressed = Input.GetKeyDown(KeyCode.KeypadDivide);
            IsMenuActionsPressed = Input.GetKeyDown(KeyCode.P);

            IsPressAutoMoveSwitchPressed = Input.GetKeyDown(KeyCode.Keypad5) || Input.GetMouseButtonDown(2);

            IsSprintButtonHolded = Input.GetButton("Sprint");


            #region Left Mouse Button

            IsMouseLeftButtonDown = Input.GetMouseButtonDown(0);
            if (IsMouseLeftButtonDown)
            {
                HowLongMouseLeftButtonHolded = 0;
                MouseLeftButtonStartHoldOnGUI = EventSystem.current.IsPointerOverGameObject();
            }

            IsMouseLeftButtonUp = Input.GetMouseButtonUp(0);

            IsMouseLeftButtonHolded = Input.GetMouseButton(0);
            if (IsMouseLeftButtonHolded)
                HowLongMouseLeftButtonHolded += Time.deltaTime;

            #endregion

            #region Right Mouse Button

            IsMouseRightButtonDown = Input.GetMouseButtonDown(1);
            if (IsMouseRightButtonDown)
            {
                HowLongMouseRightButtonHolded = 0;
                MouseRightButtonStartHoldOnGUI = EventSystem.current.IsPointerOverGameObject();
            }

            IsMouseRightButtonUp = Input.GetMouseButtonUp(1);

            IsMouseRightButtonHolded = Input.GetMouseButton(1);
            if (IsMouseRightButtonHolded)
                HowLongMouseRightButtonHolded += Time.deltaTime;

            #endregion


            SelectPlayerCharacterButtonDown = Input.GetKeyDown(KeyCode.F1);
        }
    }
}