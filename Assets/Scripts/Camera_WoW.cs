using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Assets.Scripts.Creatures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Assets.Scripts
{
    public class Camera_WoW : MonoBehaviour
    {
        private const float INPUT_X_AXIS_MIN_MAX = 70.0f;
        private const float INPUT_Y_AXIS_MIN_MAX = 70.0f;

        private const float CAM_Y_ANGLE_MIN = -40.0f;
        private const float CAM_Y_ANGLE_MAX = 89.0f;

        public enum CameraModeEnum
        {
            FollowPlayerBack,
            FollowPlayerFree,
            Cinematic
        }
        
        [Header("Camera Distance")]
        public float StartupCameraDistance = 5.0f;
        [SerializeField] private float DesiredCameraDistance;
        
        public float MinCameraDistance = 0.5f;
        public float MaxCameraDistance = 5.5f;
        public float CameraDistanceChangeSpeed = 1.5f;
        
        private float _currentCameraDistance;
        [Header("General")]

        [SerializeField] public CameraModeEnum CameraMode = CameraModeEnum.FollowPlayerBack;

        private Camera _camera;
        private IPlayerController _playerController;

        public float CameraInputXBeforeUpdate { get; private set; }
        public float CameraInputYBeforeUpdate { get; private set; }

        [SerializeField] private float _currentCameraInputX;
        public float CurrentCameraInputX
        {
            get => _currentCameraInputX;
            private set
            {
                while (value >= 360)
                    value -= 360;

                while (value < 0)
                    value += 360;

                _currentCameraInputX = value;
            }
        }

        [SerializeField] private float _currentCameraInputY;
        public float CurrentCameraInputY
        {
            get => _currentCameraInputY;
            private set { _currentCameraInputY = value; }
        }

        public float CameraStartOffsetY = 25.0f; // a bit from above

        /// <summary>
        /// Basically, distance from foot to center of head.
        /// </summary>
        public float VerticalOffset = 1.35f;

        public float SensitivityX = 3.0f;
        public float SensitivityY = 3.0f;

        public float CameraChangePositionSpeed = 3.0f;
        public float CameraReturnToPlayerBackSpeed = 150.0f;
        public float CameraReturnToPlayerBackSpeedWhenNotMoving = 480.0f;
        
        //public Material lineMaterial;

        [Header("Debug")]
        [SerializeField] private bool _slowlyLerpToCharacterBack = false;

        private const float slowlyLerpThreshold = 0.5f;

        private Quaternion _cameraNewDesiredRotationForLateUpdate;

        #region Freeze (Lock/Unlock) Cursor
        // http://answers.unity.com/answers/1265069/view.html
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
            public static implicit operator Vector2(Point p)
            {
                return new Vector2(p.X, p.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Point pos);

        private bool _cursorIsLocked = false;
        private Point _lockedCursorPosition;
        private void LockCursor()
        {
            //Debug.Log("LockCursor()");

            Cursor.visible = false;
            GetCursorPos(out _lockedCursorPosition);

            //Cursor.visible = false;
            //GetCursorPos(out _lockedCursorPosition);
            //Cursor.lockState = CursorLockMode.Locked;

            _cursorIsLocked = true;
        }

        private void UnlockCursor()
        {
            //Debug.Log("UnlockCursor()");

            Cursor.visible = true;
            SetCursorPos(_lockedCursorPosition.X, _lockedCursorPosition.Y);

            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            //SetCursorPos(_lockedCursorPosition.X, _lockedCursorPosition.Y);

            _cursorIsLocked = false;
        }

        private void OnApplicationQuit()
        {
            if (_cursorIsLocked)
                UnlockCursor();
        }
        #endregion

        private void Start()
        {
            //var playerTagGameObject = GameObject.FindGameObjectWithTag("Player");
            //var playerTransform = playerTagGameObject.transform.parent;
            //_playerWoW = playerTransform.GetComponent<IPlayerController>();

            _playerController = GameManager.Instance.PlayerGameObject.GetComponent<IPlayerController>();

            _camera = this.GetComponent<Camera>();

            _currentCameraDistance = StartupCameraDistance;

            CurrentCameraInputX = _playerController.GetRootObjectTransform().eulerAngles.y;
            CurrentCameraInputY = CameraStartOffsetY;

            _cameraNewDesiredRotationForLateUpdate = Quaternion.Euler(CurrentCameraInputY, CurrentCameraInputX, 0);
        }

        private void UpdateCameraInput()
        {
            float inputMouseScrollWheel = 0;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                inputMouseScrollWheel = GameManager.Instance.InputController_WoW.MouseScrollWheel;
                inputMouseScrollWheel *= -1; // инверсируем, т.к. дистанция камеры это расстояние "назад" от цели
            }

            var newDesiredCameraDistance = DesiredCameraDistance + inputMouseScrollWheel * CameraDistanceChangeSpeed;

            if (newDesiredCameraDistance > MaxCameraDistance)
                newDesiredCameraDistance = MaxCameraDistance;
            else if (newDesiredCameraDistance < MinCameraDistance)
                newDesiredCameraDistance = MinCameraDistance;

            DesiredCameraDistance = newDesiredCameraDistance;
        }

        public void UpdateCamera(float playerRotationBeforeTurning)
        {
            UpdateCameraInput();

            float playerRotationAfterUpdate = _playerController.GetRootObjectTransform().eulerAngles.y;

            var oldCameraMode = CameraMode;
            bool setSlowlyLerpToCharacterBackToTrueInCurrentFrame = false;

            bool cursorVisibility = true;

            if (_playerController.IsRunning || _playerController.HorizontalInput != 0)
            {
                if (GameManager.Instance.InputController_WoW.IsMouseLeftButtonHolded && !GameManager.Instance.InputController_WoW.MouseLeftButtonStartHoldOnGUI)
                {
                    CameraMode = Camera_WoW.CameraModeEnum.FollowPlayerFree;
                }
                else
                {
                    float currentDiffBetweenCharacterAngleAndCameraAngle = Mathf.Abs(CurrentCameraInputX - playerRotationBeforeTurning);

                    if (currentDiffBetweenCharacterAngleAndCameraAngle > slowlyLerpThreshold)
                    {
                        _slowlyLerpToCharacterBack = true;
                        setSlowlyLerpToCharacterBackToTrueInCurrentFrame = true;
                    }

                    CameraMode = Camera_WoW.CameraModeEnum.FollowPlayerBack;
                }
            }
            else
            {
                CameraMode = Camera_WoW.CameraModeEnum.FollowPlayerFree;
            }

            if (!setSlowlyLerpToCharacterBackToTrueInCurrentFrame && oldCameraMode != CameraMode)
                _slowlyLerpToCharacterBack = false;

            switch (CameraMode)
            {
                case CameraModeEnum.FollowPlayerBack:

                    if (GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded && !GameManager.Instance.InputController_WoW.MouseRightButtonStartHoldOnGUI)
                    {
                        UpdateCurrentCameraInputValues();
                        cursorVisibility = false;

                        _cameraNewDesiredRotationForLateUpdate = Quaternion.Euler(CurrentCameraInputY, CurrentCameraInputX, 0);
                    }
                    else
                    {
                        if (_slowlyLerpToCharacterBack)
                        {
                            if (Mathf.Abs(CurrentCameraInputX - playerRotationBeforeTurning) < slowlyLerpThreshold)
                            {
                                CurrentCameraInputX = playerRotationAfterUpdate;
                            }
                            else
                            {
                                var delta = Mathf.DeltaAngle(CurrentCameraInputX, playerRotationBeforeTurning);

                                float rotateThisFrameToGoalValue;
                                if (_playerController.IsRunning)
                                    rotateThisFrameToGoalValue = CameraReturnToPlayerBackSpeed * Time.deltaTime;
                                else
                                    rotateThisFrameToGoalValue = CameraReturnToPlayerBackSpeedWhenNotMoving * Time.deltaTime;

                                var oldCurrentCameraInputX = CurrentCameraInputX;
                                if (delta < 0)
                                    CurrentCameraInputX -= rotateThisFrameToGoalValue;
                                else
                                    CurrentCameraInputX += rotateThisFrameToGoalValue;

                                // Быть может мы слишком сильно повернули камеру и она повернулась больше чем требуется (зашла за спину персонажа с другой стороны)
                                // Если это действительно так, надо просто переприсвоитьт позицию камеры к конечной цели - спине персонажа.
                                var newDelta = Mathf.DeltaAngle(CurrentCameraInputX, playerRotationBeforeTurning);
                                if (Mathf.Abs(delta) < Mathf.Abs(newDelta))
                                    CurrentCameraInputX = playerRotationAfterUpdate;
                            }
                        }
                        else
                        {
                            CurrentCameraInputX = playerRotationAfterUpdate;
                        }

                        float secondCurrentDiffBetweenCharacterAngleAndCameraAngle = Mathf.Abs(CurrentCameraInputX - playerRotationAfterUpdate);
                        if (secondCurrentDiffBetweenCharacterAngleAndCameraAngle < slowlyLerpThreshold)
                        {
                            _slowlyLerpToCharacterBack = false;
                        }

                        _cameraNewDesiredRotationForLateUpdate = Quaternion.Euler(CurrentCameraInputY, CurrentCameraInputX, 0);
                    }

                    break;

                case CameraModeEnum.FollowPlayerFree:

                    if ((GameManager.Instance.InputController_WoW.IsMouseLeftButtonHolded && !GameManager.Instance.InputController_WoW.MouseLeftButtonStartHoldOnGUI)
                        || (GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded && !GameManager.Instance.InputController_WoW.MouseRightButtonStartHoldOnGUI))
                    {
                        UpdateCurrentCameraInputValues();
                        cursorVisibility = false;

                        _cameraNewDesiredRotationForLateUpdate = Quaternion.Euler(CurrentCameraInputY, CurrentCameraInputX, 0);
                    }

                    break;

                default: throw new NotSupportedException(nameof(CameraModeEnum));
            }

            if (cursorVisibility && _cursorIsLocked)
                UnlockCursor();
            else if (!cursorVisibility && !_cursorIsLocked)
                LockCursor();

            //GameManager.Instance.GUIManager.UpdateUIVariables(0, $"(after Update) currentCameraInputX:\t{CurrentCameraInputX}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(1, $"(after Update) playerWoW.eulerAngles.y:\t{_playerWoW.GetTransform().eulerAngles.y}");
        }

        private void UpdateCurrentCameraInputValues()
        {
            float inputXAxis = GameManager.Instance.InputController_WoW.MouseInput.x * SensitivityX;
            float inputYAxis = GameManager.Instance.InputController_WoW.MouseInput.y * SensitivityY;

            inputXAxis = Mathf.Clamp(inputXAxis, -INPUT_X_AXIS_MIN_MAX, INPUT_X_AXIS_MIN_MAX);
            inputYAxis = Mathf.Clamp(inputYAxis, -INPUT_Y_AXIS_MIN_MAX, INPUT_Y_AXIS_MIN_MAX);

            CameraInputXBeforeUpdate = CurrentCameraInputX;
            CameraInputYBeforeUpdate = CurrentCameraInputY;

            CurrentCameraInputX += inputXAxis;
            CurrentCameraInputY += inputYAxis;

            CurrentCameraInputY = Mathf.Clamp(CurrentCameraInputY, CAM_Y_ANGLE_MIN, CAM_Y_ANGLE_MAX);
        }

        private void LateUpdateCameraPosition()
        {
            _currentCameraDistance = Mathf.Lerp(_currentCameraDistance, DesiredCameraDistance, CameraChangePositionSpeed * Time.deltaTime);
        }

        private void LateUpdate()
        {
            this.LateUpdateCameraPosition();

            Vector3 actualVerticalOffset = _playerController.GetRootObjectTransform().position + new Vector3(0, VerticalOffset, 0);

            Vector3 camNewPositionRotation = _cameraNewDesiredRotationForLateUpdate * new Vector3(0, 0, -1 * _currentCameraDistance);
            Vector3 cameraNewDesiredPositionWithOffset = camNewPositionRotation + actualVerticalOffset;

            _camera.transform.position = cameraNewDesiredPositionWithOffset;
            _camera.transform.LookAt(actualVerticalOffset);
        }

        // private void OnPostRender()
        // {
        //     //RenderLines();
        // }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            //RenderLines();
        }

        //private void RenderLines()
        //{
        //    if (!lineMaterial)
        //    {
        //        Debug.LogError("Please Assign a material on the inspector");
        //        return;
        //    }

        //    if (_playerWoW.IsRunning)
        //    {
        //        GL.Begin(GL.LINES);
        //        lineMaterial.SetPass(0);
        //        GL.Color(lineMaterial.color);

        //        var initialPosition = _playerWoW.GetTransform().position + new Vector3(0, 1.51f, 0);

        //        var startPos = initialPosition;
        //        var endPos = initialPosition + _playerWoW.CurrentMoveDirectionVector;

        //        GL.Vertex(startPos);
        //        GL.Vertex(endPos);
        //        GL.End();
        //    }
        //}
    }
}
