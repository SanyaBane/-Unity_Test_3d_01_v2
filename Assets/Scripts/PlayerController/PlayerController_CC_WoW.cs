using Assets.Scripts.HelpersUnity;
using UnityEngine;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Abilities.Controller;

namespace Assets.Scripts.PlayerController
{
    [RequireComponent(typeof(GroundedInfo))]
    public class PlayerController_CC_WoW : MonoBehaviour, IPlayerController
    {
        #region Fields

        private CreatureInfoContainer _creatureInfoContainer;

        private Camera _cameraMain;
        private Camera_WoW _cameraScript;
        private GroundedInfo _groundedInfo;

        #region Running

        [Header("Running")]
        public float WalkingSpeed = 2f;

        public float RunningSpeed = 6f;
        public float RunningSpeedSprintMultiplier = 3f;
        public bool WalkMode = false;
        public bool AutoMoveForwardMode = false;

        [SerializeField] private bool _IsRunning;

        public bool IsRunning
        {
            get => _IsRunning;
            set { _IsRunning = value; }
        }

        [Header("Helpers")]
        [SerializeField] private GameObject _moveDirectionAxisHelper;

        [SerializeField] private bool _showMoveDirectionAxisHelper = false;

        [SerializeField] private GameObject _groundDirectionAxisHelper;
        [SerializeField] private bool _showGroundDirectionAxisHelper = false;

        [SerializeField] private GameObject _fallDirectionAxisHelper;
        [SerializeField] private bool _showFallDirectionAxisHelper = false;

        #endregion // Running

        #region Gravity and Velocity_Y

        [Header("Gravity and Velocity_Y")]
        public float Gravity = 24.0f; // 9.8f ???

        public float MaxNegativeVelocityY = -12.0f;
        private float _velocity_Y = 0;

        #endregion // Gravity and Velocity_Y

        #region Turning

        [Header("Turning")]
        private bool _IsTurningByMouse = false;

        public bool IsTurningByMouse => _IsTurningByMouse;

        private bool _IsTurningByKeyboard = false;
        public bool IsTurningByKeyboard => _IsTurningByKeyboard;

        public float TurnSpeed = 2.0f;

        #endregion // Turning

        #region Jumping

        [Header("Jumping")]
        public float JumpForce = 8.0f;

        public float DoubleJumpForce = 8.0f;

        private bool _isJumpPressedThisFrame;

        private readonly float _velocityWhenStartedToFallWithoutJumping = 2.0f;

        /// <summary>
        /// Set to negative value to allow infinite jumps.
        /// </summary>
        public int maxJumpsLimit = 1;

        [SerializeField] private int _howManyJumpsPerformed = 0;
        private bool _isDisableDoubleJumpIfTooMuchNegativeVelocity = false; // probably should remain "false"
        private readonly float _negativeVelocityThresholdForDoubleJump = -5.0f;

        #endregion // Jumping

        #region Animator stuff

        private float _animatorVelocityChangeSpeed = 10.0f;

        private AnimatorPlayerData _animatorPlayerData;

        private float _animatorInputY = 0;
        private float _animatorInputX = 0;

        private readonly float _animVelocityXValueMax = 1.0f;
        private readonly float _animVelocityYValueMax = 1.0f;

        #endregion // Animator stuff

        private Vector3 _playerPositionOnLastCollision = Vector3.zero;
        private Vector3 collisionPoint = Vector3.zero;

        #endregion // Fields

        #region Properties

        private PlayerCreature _playerCreature => _creatureInfoContainer.BaseCreature as PlayerCreature;
        private PlayerHealth _playerHealth => _creatureInfoContainer.BaseCreature.Health as PlayerHealth;
        private AbilitiesController _abilitiesController => _creatureInfoContainer.BaseCreature.AbilitiesController;
        public Animator Animator => _creatureInfoContainer.BaseCreature.Animator;
        public CharacterController CharacterController { get; private set; }
        public Vector3 PlayerPositionLastFrame { get; private set; } = Vector3.zero;

        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }
        public float StrafeInput { get; private set; }

        #endregion // Properties
        
        protected void Awake()
        {
            //Application.targetFrameRate = 8;

            _creatureInfoContainer = GetComponent<CreatureInfoContainer>();
            _groundedInfo = GetComponent<GroundedInfo>();

            CharacterController = GetComponent<CharacterController>();
            if (CharacterController == null)
                Debug.LogError($"{nameof(CharacterController)} == null");

            _cameraMain = Camera.main;
            _cameraScript = _cameraMain.GetComponent<Camera_WoW>();

            _animatorPlayerData = new AnimatorPlayerData();
            _animatorPlayerData.ResetAnimatorData();
        }

        protected void Start()
        {
            if (Animator == null)
                Debug.LogError($"{nameof(Animator)} == null");
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false) //  || EditorApplication.isPaused
                return;

            //if (_characterController == null)
            //{
            //    Debug.LogWarning("Getting CharacterController in editor mode");
            //    _characterController = GetComponent<CharacterController>();
            //}


            // IsGroundedSphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundedInfo.IsGroundedSpherePosition, _groundedInfo.IsGroundedSphereRadius);


            //if (_lastGroundedCharControllerColliderHitInfo.Count > 0)
            //{
            //    Vector3 averageVectorPoint = VectorHelper.GetMeanVector(_lastGroundedCharControllerColliderHitInfo.Select(x => x.point - _playerPositionOnLastCollision + _playerCreature.GetTransform().position).ToArray());
            //    Vector3 averageVectorNormal = VectorHelper.GetMeanVector(_lastGroundedCharControllerColliderHitInfo.Select(x => x.normal).ToArray());

            //    Gizmos.color = Color.yellow;
            //    foreach (var element in _lastGroundedCharControllerColliderHitInfo)
            //    {
            //        var pointRelativeToCurrentPos = element.point - _playerPositionOnLastCollision + _playerCreature.GetTransform().position;

            //        Gizmos.DrawSphere(pointRelativeToCurrentPos, 0.01f);
            //        Gizmos.DrawRay(pointRelativeToCurrentPos, element.normal);
            //    }

            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(averageVectorPoint, 0.01f);
            //    Gizmos.DrawRay(averageVectorPoint, averageVectorNormal);
            //}
        }

        protected void Update()
        {
            if (GameManager.Instance.InputController_WoW.IsNumpadSlashPressed)
                WalkMode = !WalkMode;

            if (GameManager.Instance.InputController_WoW.IsPressAutoMoveSwitchPressed)
                AutoMoveForwardMode = !AutoMoveForwardMode;

            _isJumpPressedThisFrame = Input.GetButtonDown("Jump");

            Locomotion();

            _playerCreature.IsGrounded = _groundedInfo.IsGroundedByCharController;

            _playerCreature.IsMoving = this.IsRunning || (PlayerPositionLastFrame != GetRootObjectTransform().position);
            // _playerCreature.IsMoving = this.IsRunning || (this.IsGroundedByCharController == false || this.IsGroundedBySphere == false);
            // Debug.Log($"_playerCreature.IsMoving: {_playerCreature.IsMoving}; IsRunning: {IsRunning}; IsGroundedByCharController: {IsGroundedByCharController};");

            _playerCreature.IsTurning = this.IsTurningByMouse || this.IsTurningByKeyboard;

            PlayerPositionLastFrame = GetRootObjectTransform().position;
        }

        private void Locomotion()
        {
            _groundedInfo.UpdateIsGrounded();

            var playerRotationBeforeTurning = GetRootObjectTransform().eulerAngles.y;

            _IsTurningByMouse = false;
            _IsTurningByKeyboard = false;

            IsRunning = false;


            VerticalInput = GameManager.Instance.InputController_WoW.Vertical;
            HorizontalInput = GameManager.Instance.InputController_WoW.Horizontal;
            StrafeInput = GameManager.Instance.InputController_WoW.Strafe;

            // Если нажаты обе кнопки мыши
            if (GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded && GameManager.Instance.InputController_WoW.IsMouseLeftButtonHolded)
                VerticalInput += 1;

            _animatorPlayerData.ResetAnimatorData();

            if (_playerHealth.IsAlive || _playerHealth.CanMoveWhenDead)
                Turn();

            Move();

            HandleCharacterRootVerticalPosition();

            _cameraScript.UpdateCamera(playerRotationBeforeTurning);
        }

        private void HandleCharacterRootVerticalPosition()
        {
            var localPosition = _creatureInfoContainer.CharacterMeshRoot.transform.localPosition;
            localPosition = new Vector3(localPosition.x, CharacterController.skinWidth * -1, localPosition.z);
            _creatureInfoContainer.CharacterMeshRoot.transform.localPosition = localPosition;
        }

        private void Turn()
        {
            // если во время режима автобега нажаты клавиши вперёд / назад, отменять режим автобега
            if (AutoMoveForwardMode && VerticalInput != 0)
                AutoMoveForwardMode = false;

            if (GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded && GameManager.Instance.InputController_WoW.MouseRightButtonStartHoldOnGUI == false)
            {
                if (_abilitiesController.CastAbilityCoroutineWrapper.IsInProgress == false)
                {
                    if ((GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded && GameManager.Instance.InputController_WoW.IsMouseLeftButtonHolded) // Если нажаты обе кнопки мыши
                        || _cameraScript.CameraInputXBeforeUpdate != _cameraScript.CurrentCameraInputX) // или камера повернулась по оси Y (по сравнению с предыдущим сохранённым значением)
                    {
                        var newDesiredRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _cameraMain.transform.eulerAngles.y, transform.rotation.eulerAngles.z);
                        transform.rotation = newDesiredRotation;

                        _IsTurningByMouse = true;
                    }
                }
            }
            else
            {
                if (Mathf.Abs(HorizontalInput) > 0)
                {
                    float rotationAngle = 0;

                    _IsTurningByKeyboard = true;

                    if (HorizontalInput > 0) //only right
                    {
                        rotationAngle = GetRootObjectTransform().rotation.eulerAngles.y + TurnSpeed * Time.deltaTime;
                    }
                    else if (HorizontalInput < 0) //only left
                    {
                        rotationAngle = GetRootObjectTransform().rotation.eulerAngles.y - TurnSpeed * Time.deltaTime;
                    }

                    Quaternion newDesiredRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationAngle, transform.rotation.eulerAngles.z);

                    transform.rotation = newDesiredRotation;
                }
            }
        }

        private Vector3 CalculateMoveDirection(float forwardMult, float strafeMult, Transform directionTransform)
        {
            Vector3 moveDirectionWorldVector = Vector3.zero;

            IsRunning = false;

            if (_playerHealth.IsAlive || _playerHealth.CanMoveWhenDead)
            {
                if (AutoMoveForwardMode)
                {
                    moveDirectionWorldVector += directionTransform.forward * forwardMult;
                    _animatorPlayerData.IsRunningForward = true;
                }
                else if (Mathf.Abs(VerticalInput) > 0)
                {
                    if (VerticalInput > 0)
                    {
                        moveDirectionWorldVector += directionTransform.forward * forwardMult;
                        _animatorPlayerData.IsRunningForward = true;
                    }
                    else
                    {
                        moveDirectionWorldVector -= directionTransform.forward * forwardMult;
                        _animatorPlayerData.IsRunningBackward = true;
                    }
                }

                if (Mathf.Abs(HorizontalInput) > 0 && GameManager.Instance.InputController_WoW.IsMouseRightButtonHolded)
                {
                    if (HorizontalInput > 0)
                    {
                        moveDirectionWorldVector += directionTransform.right * strafeMult;
                        _animatorPlayerData.IsRunningStrafeRight = true;
                    }
                    else
                    {
                        moveDirectionWorldVector -= directionTransform.right * strafeMult;
                        _animatorPlayerData.IsRunningStrafeLeft = true;
                    }
                }
                else if (Mathf.Abs(StrafeInput) > 0)
                {
                    if (StrafeInput > 0)
                    {
                        moveDirectionWorldVector += directionTransform.right * strafeMult;
                        _animatorPlayerData.IsRunningStrafeRight = true;
                    }
                    else
                    {
                        moveDirectionWorldVector -= directionTransform.right * strafeMult;
                        _animatorPlayerData.IsRunningStrafeLeft = true;
                    }
                }
            }

            IsRunning = (_animatorPlayerData.IsRunningForward || _animatorPlayerData.IsRunningBackward || _animatorPlayerData.IsRunningStrafeRight || _animatorPlayerData.IsRunningStrafeLeft) 
                        && _groundedInfo.SlidingDownTheSlope == false;

            //if (_groundedInfo.SlidingDownTheSlope)
            //{
            //    moveDirectionWorldVector = Vector3.zero;
            //}

            Vector3 moveDirectionNormalizedWorldVector = moveDirectionWorldVector;

            if (moveDirectionNormalizedWorldVector.magnitude > 1.0f)
                moveDirectionNormalizedWorldVector = moveDirectionNormalizedWorldVector.normalized;

            return moveDirectionNormalizedWorldVector;
        }

        private void Move()
        {
            Vector3 moveDirectionWithoutMultipliers = CalculateMoveDirection(1, 1, this.transform);

            // CorrectAnimationVelocityAndSendItToAnimator();

            GameManager.Instance.GUIManager.UpdateUIVariables(1, $"_velocity_Y:\t{_velocity_Y}");

            Vector3 currentCharacterDirection;
            if (moveDirectionWithoutMultipliers != Vector3.zero)
                currentCharacterDirection = moveDirectionWithoutMultipliers;
            else
                currentCharacterDirection = GetRootObjectTransform().forward;

            // Берём для groundDirection те же значения поворота что и у игрока.
            // Так как оно всегда будет смотреть "вперёд", как и персонаж, значит к нему можно будет корректно применять "forwardMult" и "strafeMult".
            _groundDirectionAxisHelper.transform.rotation = GetRootObjectTransform().rotation;
            _fallDirectionAxisHelper.transform.rotation = GetRootObjectTransform().rotation;

            float forwardMult = 1;
            float strafeMult = 1;
            float fallMult = 1;

            _groundedInfo.IsGroundedByRaycast = false;

            Vector3 charControllerColliderHitPoint;

            if (_groundedInfo.IsGroundedByCharController)
                charControllerColliderHitPoint = collisionPoint;
            else
                charControllerColliderHitPoint = GetRootObjectTransform().position;

            float groundRayOffsetUp = 0.05f;
            var groundRayOrigin = charControllerColliderHitPoint + Vector3.up * groundRayOffsetUp;
            var groundRayDirection = Vector3.down;
            Ray groundRay = new Ray(groundRayOrigin, groundRayDirection);

            float groundRayDistance = 0.3f;


            _groundedInfo.SlidingDownTheSlope = false;

            _groundedInfo.IsGroundedByRaycast = Physics.Raycast(groundRay, out RaycastHit groundHit, groundRayDistance, _groundedInfo.GroundMask);
            if (_groundedInfo.IsGroundedByRaycast)
            {
                Vector3 groundHitNormal = groundHit.normal;

                float slopeAngle = Vector3.Angle(Vector3.up, groundHitNormal);
                slopeAngle = Mathf.Round(slopeAngle);

                float directionAngle = Vector3.Angle(currentCharacterDirection, groundHitNormal) - 90;

                GameManager.Instance.GUIManager.UpdateUIVariables(2, $"slopeAngle:\t{slopeAngle}");
                GameManager.Instance.GUIManager.UpdateUIVariables(3, $"directionAngle:\t{directionAngle}");

                float forwardAngle = Vector3.Angle(transform.forward, groundHitNormal) - 90;
                forwardMult = 1 / Mathf.Cos(forwardAngle * Mathf.Deg2Rad);

                float strafeAngle = Vector3.Angle(transform.right, groundHitNormal) - 90;
                strafeMult = 1 / Mathf.Cos(strafeAngle * Mathf.Deg2Rad);

                _groundDirectionAxisHelper.transform.eulerAngles += new Vector3(-forwardAngle, 0, 0);
                _groundDirectionAxisHelper.transform.eulerAngles += new Vector3(0, 0, strafeAngle);

                if (slopeAngle <= CharacterController.slopeLimit)
                {
                    //_groundDirectionAxisHelper.transform.eulerAngles += new Vector3(-forwardAngle, 0, 0);
                    //_groundDirectionAxisHelper.transform.eulerAngles += new Vector3(0, 0, strafeAngle);
                }
                else
                {
                    _groundedInfo.SlidingDownTheSlope = true;

                    fallMult = 1 / Mathf.Cos((90 - slopeAngle) * Mathf.Deg2Rad);

                    Vector3 groundCross = Vector3.Cross(groundHitNormal, Vector3.up);
                    Vector3 tmpCross = Vector3.Cross(groundCross, groundHitNormal);
                    _fallDirectionAxisHelper.transform.rotation = Quaternion.FromToRotation(transform.up, tmpCross);
                }
            }
            else
            {
                GameManager.Instance.GUIManager.UpdateUIVariables(2, $"_slopeAngle:\tnull");
                GameManager.Instance.GUIManager.UpdateUIVariables(3, $"directionAngle:\tnull");
            }


            GameManager.Instance.GUIManager.UpdateUIVariables(4, $"fallMult:\t{fallMult}");

            Vector3 moveDirectionWithMultipliers = CalculateMoveDirection(forwardMult, strafeMult, _groundDirectionAxisHelper.transform);

            _moveDirectionAxisHelper.transform.LookAt(this.transform.position + moveDirectionWithMultipliers);

            #region Helpers

            if (_moveDirectionAxisHelper != null)
            {
                _moveDirectionAxisHelper.SetActive(_showMoveDirectionAxisHelper);
            }

            if (_groundDirectionAxisHelper != null)
            {
                _groundDirectionAxisHelper.SetActive(_showGroundDirectionAxisHelper && _groundedInfo.IsGroundedByRaycast /*IsGroundedByCharController*/);
            }

            if (_fallDirectionAxisHelper != null)
            {
                _fallDirectionAxisHelper.SetActive(_showFallDirectionAxisHelper);
            }

            #endregion

            float speedToUse = GetSpeed();

            GameManager.Instance.GUIManager.UpdateUIVariables(6, $"speedToUse:\t{speedToUse}");
            GameManager.Instance.GUIManager.UpdateUIVariables(7, $"moveDirectionWithMultipliers after(?) modification:\t{moveDirectionWithMultipliers}");

            Vector3 velocity = moveDirectionWithMultipliers * speedToUse;

            ProcessGravityVelocity(); // calculation of "_velocity_Y"
            velocity += _fallDirectionAxisHelper.transform.up * (_velocity_Y * fallMult);

            //var verticalVelocity = CalculateVerticalVelocity(_fallDirectionAxisHelper.transform.up);
            //velocity += verticalVelocity;

            //GameManager.Instance.GUIManager.UpdateUIVariables(8, $"_fallDirectionAxisHelper.transform.up:\t{_fallDirectionAxisHelper.transform.up}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(9, $"_velocity_Y:\t{_velocity_Y}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(10, $"velocity:\t{velocity}");
            //GameManager.Instance.GUIManager.UpdateUIVariables(11, $"velocity.magnitude:\t{velocity.magnitude}");

            CollisionFlags moveResult = CharacterController.Move(velocity * Time.deltaTime); // собственно, двигаем персонажа
            //GameManager.Instance.GUIManager.UpdateUIVariables(10, $"moveResult:\t{moveResult}");

            HandleJumpingToCeiling(); // Прыжок головой об потолок

            CorrectAnimationVelocityAndSendItToAnimator();
        }


        private float GetSpeed()
        {
            float speed;

            if (GameManager.Instance.InputController_WoW.IsSprintButtonHolded) // sprint
            {
                speed = RunningSpeed;
                speed *= RunningSpeedSprintMultiplier;
                return speed;
            }

            if (WalkMode || _animatorPlayerData.IsRunningBackward)
            {
                speed = WalkingSpeed;
            }
            else
            {
                speed = RunningSpeed;
            }

            return speed;
        }

        private void ProcessGravityVelocity()
        {
            bool playerCanJump = (maxJumpsLimit < 0 || _howManyJumpsPerformed < maxJumpsLimit) &&
                                 (_playerHealth.IsAlive || (_playerHealth.IsAlive == false && _playerHealth.CanMoveWhenDead));

            if (_groundedInfo.IsGroundedByCharController) //&& (moveResult & CollisionFlags.CollidedBelow) > 0)
            {
                _howManyJumpsPerformed = 0;

                // Reset velocity_Y to "zero" only if we are sure that character not sliding from slope 
                // (when he do this, he sometimes not grounded, so we need this extra check)
                if (_groundedInfo.SlidingDownTheSlope == false)
                {
                    _velocity_Y = 0;
                }

                _velocity_Y -= Gravity * Time.deltaTime;

                // press jump when staying on ground
                if (_isJumpPressedThisFrame && playerCanJump)
                {
                    _animatorPlayerData.JumpStart = true;

                    _velocity_Y = JumpForce;

                    //_isGrounded = false;
                    _howManyJumpsPerformed++;
                }
            }
            else
            {
                if (_isJumpPressedThisFrame && playerCanJump)
                {
                    if (_isDisableDoubleJumpIfTooMuchNegativeVelocity)
                    {
                        if (_velocity_Y > _negativeVelocityThresholdForDoubleJump)
                        {
                            _velocity_Y = DoubleJumpForce;
                            _howManyJumpsPerformed++;
                        }
                    }
                    else
                    {
                        _velocity_Y = DoubleJumpForce;
                        _howManyJumpsPerformed++;
                    }
                }
                else
                {
                    // если velocity_Y идеально равно нулю и "isGrounded == false", скорей всего персонаж падает, не совершив перед этим прыжок
                    // в таком случае я хочу дать ему немного более ускоренное (в самом начале), падение.
                    // Иначе возможен вариант что он будет падать настолько медленно, что успеет "перешагнуть" через короткую пропасть
                    if (_velocity_Y == 0)
                    {
                        _velocity_Y -= _velocityWhenStartedToFallWithoutJumping;
                    }
                    else
                    {
                        _velocity_Y -= Gravity * Time.deltaTime;
                    }
                }
            }

            // ограничиваем максимально возможное негативное значение Velocity_Y
            if (_velocity_Y < MaxNegativeVelocityY)
                _velocity_Y = MaxNegativeVelocityY;
        }

        private void HandleJumpingToCeiling()
        {
            // если персонаж прыгнул с положительной "_velocity_Y" и ударился об потолок, надо это отследить и моментально изменить "_velocity_Y" на "0"
            // иначе персонаж продолжит "парить" в воздухе пока "_velocity_Y" постепенно не станет "<= 0"
            if (_groundedInfo.IsGroundedByCharController == false && _velocity_Y > 0 && GetRootObjectTransform().position.y == PlayerPositionLastFrame.y)
            {
                // При очень низком Velocity_Y, условие выше способно выполняться даже во время обычного прыжка. 
                // То есть просто иногда так получается что даже при позитивном, но крайне маленьким, Velocity_Y, два кадра подряд позиция игрока по оси Y не меняется.
                // Значит мы должны объявить Threshold с определённым значением и использовать его.

                float tempVelocityYThreshold = 0.1f; // 0.1f

                if (_velocity_Y > tempVelocityYThreshold)
                {
                    //Debug.Log($"Character hit ceiling with his head... _velocity_Y: {_velocity_Y}.");
                    _velocity_Y = 0;
                }
            }
        }

        private void CorrectAnimationVelocityAndSendItToAnimator()
        {
            float animVelocityChangeSpeedWithTimeApplied = _animatorVelocityChangeSpeed * Time.deltaTime;

            #region Forward / Backward

            if (_animatorPlayerData.IsRunningForward || _animatorPlayerData.IsRunningBackward)
            {
                if (_animatorPlayerData.IsRunningForward)
                    _animatorInputY += animVelocityChangeSpeedWithTimeApplied;
                else
                    _animatorInputY -= animVelocityChangeSpeedWithTimeApplied;

                _animatorInputY = Mathf.Clamp(_animatorInputY, _animVelocityYValueMax * -1, _animVelocityYValueMax);
            }
            else
            {
                if (_animatorInputY < 0)
                {
                    _animatorInputY = Mathf.Min(_animatorInputY + animVelocityChangeSpeedWithTimeApplied, 0);
                }
                else if (_animatorInputY > 0)
                {
                    _animatorInputY = Mathf.Max(_animatorInputY - animVelocityChangeSpeedWithTimeApplied, 0);
                }
            }

            #endregion

            #region Left / Right

            if (_animatorPlayerData.IsRunningStrafeLeft || _animatorPlayerData.IsRunningStrafeRight)
            {
                if (_animatorPlayerData.IsRunningStrafeRight)
                    _animatorInputX += animVelocityChangeSpeedWithTimeApplied;
                else
                    _animatorInputX -= animVelocityChangeSpeedWithTimeApplied;

                _animatorInputX = Mathf.Clamp(_animatorInputX, _animVelocityXValueMax * -1, _animVelocityXValueMax);
            }
            else
            {
                if (_animatorInputX < 0)
                {
                    _animatorInputX = Mathf.Min(_animatorInputX + animVelocityChangeSpeedWithTimeApplied, 0);
                }
                else if (_animatorInputX > 0)
                {
                    _animatorInputX = Mathf.Max(_animatorInputX - animVelocityChangeSpeedWithTimeApplied, 0);
                }
            }

            #endregion

            float characterRootTurnSpeed = 10.0f; // probably should be class-variable

            float characterRootGoalYRotation;
            if (_animatorPlayerData.IsRunningBackward && _groundedInfo.IsGroundedByCharController)
                characterRootGoalYRotation = _animatorInputX * -45;
            else
                characterRootGoalYRotation = 0;

            if (_creatureInfoContainer.CharacterMeshRoot.transform.localRotation.eulerAngles.y != characterRootGoalYRotation)
            {
                float lerpedGoalYRotation = Mathf.LerpAngle(_creatureInfoContainer.CharacterMeshRoot.transform.localRotation.eulerAngles.y, characterRootGoalYRotation, characterRootTurnSpeed * Time.deltaTime);
                _creatureInfoContainer.CharacterMeshRoot.transform.localRotation = Quaternion.Euler(0, lerpedGoalYRotation, 0);
            }

            Animator.SetFloat(ConstantsAnimator.PLAYER_CONTROLLER_FLOAT_INPUT_X, _animatorInputX);
            Animator.SetFloat(ConstantsAnimator.PLAYER_CONTROLLER_FLOAT_INPUT_Y, _animatorInputY);

            Animator.SetBool(ConstantsAnimator.PLAYER_CONTROLLER_BOOL_IS_RUNNING, IsRunning);
            Animator.SetFloat(ConstantsAnimator.PLAYER_CONTROLLER_FLOAT_HOW_LONG_GROUNDED, _groundedInfo.HowLongGrounded);
            Animator.SetFloat(ConstantsAnimator.PLAYER_CONTROLLER_FLOAT_HOW_LONG_NOT_GROUNDED, _groundedInfo.HowLongNotGrounded);

            Animator.SetBool(ConstantsAnimator.PLAYER_CONTROLLER_BOOL_JUMP_START, _animatorPlayerData.JumpStart);

            Animator.SetBool(ConstantsAnimator.PLAYER_CONTROLLER_BOOL_IS_GROUNDED, _groundedInfo.IsGroundedByCharController && _groundedInfo.SlidingDownTheSlope == false);
        }

        void OnControllerColliderHit(ControllerColliderHit controllerColliderHit)
        {
            //Debug.Log("OnControllerColliderHit()");

            //collisionPoint = controllerColliderHit.point;

            if (controllerColliderHit.point.y <= transform.position.y + 0.25f)
            {
                if (_groundedInfo.IsGroundedBySphere)
                {
                    float gDistanceSquared = VectorHelper.DistanceSquared(_groundedInfo.IsGroundedSpherePosition, controllerColliderHit.point);
                    if (gDistanceSquared <= Mathf.Pow(_groundedInfo.IsGroundedSphereRadius, 2))
                    {
                        collisionPoint = controllerColliderHit.point;
                    }
                }
                else
                {
                    collisionPoint = controllerColliderHit.point;
                }
            }

            _playerPositionOnLastCollision = GetRootObjectTransform().position;
        }

        public Transform GetRootObjectTransform() => _creatureInfoContainer.transform;

        public Vector3 GetGroundedPosition() => _playerCreature.GetGroundedPosition();
    }
}