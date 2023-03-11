using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.TestMovement
{
    public class PlayerControls : MonoBehaviour
    {
        //inputs
        public Controls controls;
        private Vector2 inputs;
        [HideInInspector] public Vector2 inputNormalized;
        [HideInInspector] public float rotation;
        private bool run = true, jump;
        [HideInInspector] public bool steer, autoRun;
        public LayerMask groundMask;

        public MoveState moveState = MoveState.locomotion;

        //velocity
        private Vector3 velocity;
        private float gravity = -18, velocityY, terminalVelocity = -25;
        private float fallMult;

        //Running
        private float currentSpeed;
        public float baseSpeed = 1, runSpeed = 4, rotateSpeed = 2;

        //ground
        private Vector3 forwardDirection, collisionPoint;
        private float slopeAngle, forwardAngle, strafeAngle;
        private float forwardMult, strafeMult;
        private Ray _groundRay;
        private RaycastHit groundHit;

        private float IsGroundedSphereRadius;
        private Vector3 IsGroundedSpherePosition;

        //Jumping
        public bool InfiniteJumps = true;
        [SerializeField] private bool jumping;
        private float jumpSpeed;
        private Vector3 jumpDirection;
        public float jumpHeight = 3;

        //swimming
        private float swimSpeed = 2, swimLevel = 1.25f;
        public float waterSurface, d_fromWaterSurface;
        public bool inWater;

        //Debug
        public bool showGroundRay, showGroundSphere, showMoveDirection, showForwardDirection, showStrafeDirection, showFallNormal, showSwimNormal;

        [SerializeField] private bool _isGroundedByCharController;
        [SerializeField] private bool _isGroundedByRaycast;
        [SerializeField] private bool _isGroundedBySphere;

        //references
        private CharacterController _characterController;
        public Transform groundDirection, moveDirection, fallDirection, swimDirection;
        [HideInInspector] public CameraController mainCam;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _isGroundedByCharController = _characterController.isGrounded;

            GetInputs();

            GroundSphereCheck();

            GetSwimDirection();

            if (inWater)
                GetWaterlevel();

            switch (moveState)
            {
                case MoveState.locomotion:
                    Locomotion();
                    break;

                case MoveState.swimming:
                    Swimming();
                    break;
            }
        }

        private void GroundSphereCheck()
        {
            float gSkin = _characterController.skinWidth * 1.2f;

            //gRadius = _characterController.radius / 2;
            IsGroundedSphereRadius = Mathf.Cos(_characterController.slopeLimit * Mathf.Deg2Rad);
            IsGroundedSphereRadius = IsGroundedSphereRadius / (1 - IsGroundedSphereRadius);
            IsGroundedSphereRadius *= gSkin;
            IsGroundedSphereRadius = _characterController.radius - IsGroundedSphereRadius;

            IsGroundedSpherePosition = transform.position + Vector3.up * (IsGroundedSphereRadius - gSkin);

            _isGroundedBySphere = Physics.CheckSphere(IsGroundedSpherePosition, IsGroundedSphereRadius, groundMask);
        }

        private void Locomotion()
        {
            CalculateDirectionsAndMultipliers();

            //running and walking
            if (_isGroundedByCharController && slopeAngle <= _characterController.slopeLimit)
            {
                currentSpeed = baseSpeed;

                if (run)
                {
                    currentSpeed *= runSpeed;

                    if (inputNormalized.y < 0)
                        currentSpeed = currentSpeed / 2;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                    currentSpeed *= 5;
            }
            else if (!_isGroundedByCharController || slopeAngle > _characterController.slopeLimit)
            {
                inputNormalized = Vector2.Lerp(inputNormalized, Vector2.zero, 0.025f);
                currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.025f);

                //Debug.Log($"currentSpeed: {currentSpeed}");
                //if (Input.GetKey(KeyCode.LeftShift))
                //    currentSpeed *= 5;
            }

            //Rotating
            Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
            transform.eulerAngles = characterRotation;

            //Press space to Jump
            if ((jump && _isGroundedByCharController && slopeAngle <= _characterController.slopeLimit && !jumping)
                || (jump && InfiniteJumps))
                Jump();

            //apply gravity if not grounded
            if (!_isGroundedByCharController && velocityY > terminalVelocity)
                velocityY += gravity * Time.deltaTime;
            else if (_isGroundedByCharController && slopeAngle > _characterController.slopeLimit)
                velocityY = Mathf.Lerp(velocityY, terminalVelocity, 0.25f);

            //checking WaterLevel
            if (inWater)
            {
                //setting ground ray
                _groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
                _groundRay.direction = Vector3.down;

                //if (Physics.Raycast(groundRay, out groundHit, 0.15f))
                //    currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, d_fromWaterSurface / swimLevel);

                if (d_fromWaterSurface >= swimLevel)
                {
                    if (jumping)
                        jumping = false;

                    moveState = MoveState.swimming;
                }
            }

            //Applying inputs
            if (jumping)
            {
                //velocity = jumpDirection * jumpSpeed + Vector3.up * velocityY;

                var speedToUse = baseSpeed * runSpeed;
                if (Input.GetKey(KeyCode.LeftShift))
                    speedToUse *= 5;

                velocity = transform.forward * inputNormalized.y + transform.right * inputNormalized.x;
                GameManager.Instance.GUIManager.UpdateUIVariables(2, $"velocity init:\t{velocity}");
                GameManager.Instance.GUIManager.UpdateUIVariables(4, $"speedToUse:\t{speedToUse}");
                velocity *= speedToUse;
                velocity += fallDirection.up * (velocityY * fallMult);
            }
            else
            {
                velocity = groundDirection.forward * inputNormalized.y * forwardMult + groundDirection.right * inputNormalized.x * strafeMult; //Applying movement direction inputs
                GameManager.Instance.GUIManager.UpdateUIVariables(2, $"velocity init:\t{velocity}");

                velocity *= currentSpeed; //Applying current move speed
                GameManager.Instance.GUIManager.UpdateUIVariables(4, $"currentSpeed:\t{currentSpeed}");

                velocity += fallDirection.up * (velocityY * fallMult); //Gravity
            }


            GameManager.Instance.GUIManager.UpdateUIVariables(0, $"velocityY:\t{velocityY}");
            GameManager.Instance.GUIManager.UpdateUIVariables(3, $"velocity:\t{velocity}");

            //moving controller
            var moveResult = _characterController.Move(velocity * Time.deltaTime);
            //Debug.Log($"moveResult: {moveResult}; velocity: {velocity}");

            if (_isGroundedByCharController)
            {
                //stop jumping if grounded
                if (jumping)
                    jumping = false;

                // stop gravity if grounded
                velocityY = 0;
            }
        }

        private void CalculateDirectionsAndMultipliers()
        {
            //SETTING FORWARDDIRECTION
            //Setting forwardDirection to controller position
            forwardDirection = transform.position;

            //Setting forwardDirection based on control input.
            if (inputNormalized.magnitude > 0)
                forwardDirection += transform.forward * inputNormalized.y + transform.right * inputNormalized.x;
            else
                forwardDirection += transform.forward;

            //Setting groundDirection to look in the forwardDirection normal
            moveDirection.LookAt(forwardDirection);
            fallDirection.rotation = transform.rotation;
            groundDirection.rotation = transform.rotation;


            //setting ground ray
            float groundRayOffsetUp = 0.05f;
            _groundRay.origin = transform.position + collisionPoint + Vector3.up * groundRayOffsetUp;
            _groundRay.direction = Vector3.down;

            float groundRayDistance = 0.3f;
            if (showGroundRay)
                Debug.DrawLine(_groundRay.origin, _groundRay.origin + Vector3.down * groundRayDistance, Color.red);

            forwardMult = 1;
            fallMult = 1;
            strafeMult = 1;

            _isGroundedByRaycast = Physics.Raycast(_groundRay, out groundHit, groundRayDistance, groundMask);
            if (_isGroundedByRaycast)
            {
                //Getting angles
                slopeAngle = Vector3.Angle(transform.up, groundHit.normal);
                var directionAngle = Vector3.Angle(moveDirection.forward, groundHit.normal) - 90;

                if (directionAngle < 0 && slopeAngle <= _characterController.slopeLimit)
                {
                    //forwardAngle = Vector3.Angle(transform.forward, groundHit.normal) - 90; //Chekcing the forwardAngle against the slope
                    forwardAngle = Vector3.Angle(groundDirection.forward, groundHit.normal) - 90; //Chekcing the forwardAngle against the slope
                    forwardMult = 1 / Mathf.Cos(forwardAngle * Mathf.Deg2Rad); //Applying the forward movement multiplier based on the forwardAngle
                    groundDirection.eulerAngles += new Vector3(-forwardAngle, 0, 0); //Rotating groundDirection X

                    strafeAngle = Vector3.Angle(groundDirection.right, groundHit.normal) - 90; //Checking the strafeAngle against the slope
                    strafeMult = 1 / Mathf.Cos(strafeAngle * Mathf.Deg2Rad); //Applying the strafe movement multiplier based on the strafeAngle
                    groundDirection.eulerAngles += new Vector3(0, 0, strafeAngle);
                }
                else if (slopeAngle > _characterController.slopeLimit)
                {
                    float groundDistance = Vector3.Distance(_groundRay.origin, groundHit.point);

                    if (groundDistance <= 0.1f)
                    {
                        fallMult = 1 / Mathf.Cos((90 - slopeAngle) * Mathf.Deg2Rad);

                        Vector3 groundCross = Vector3.Cross(groundHit.normal, Vector3.up);
                        fallDirection.rotation = Quaternion.FromToRotation(transform.up, Vector3.Cross(groundCross, groundHit.normal));
                    }
                }
            }

            GameManager.Instance.GUIManager.UpdateUIVariables(8, $"velocity:\t{velocity}");
            GameManager.Instance.GUIManager.UpdateUIVariables(6, $"isGroundedByCharController:\t{_isGroundedByCharController}");
            GameManager.Instance.GUIManager.UpdateUIVariables(7, $"isGroundedByRaycast:\t{_isGroundedByRaycast}");

            DebugGroundNormals();
        }

        private void Jump()
        {
            //set Jumping to true
            if (!jumping)
                jumping = true;

            switch (moveState)
            {
                case MoveState.locomotion:
                    //Set jump direction and speed
                    jumpDirection = (transform.forward * inputs.y + transform.right * inputs.x).normalized;
                    jumpSpeed = currentSpeed;

                    //set velocity Y
                    velocityY = Mathf.Sqrt(-gravity * jumpHeight);
                    break;

                case MoveState.swimming:
                    //Set jump direction and speed
                    jumpDirection = (transform.forward * inputs.y + transform.right * inputs.x).normalized;
                    jumpSpeed = swimSpeed;

                    //set velocity Y
                    velocityY = Mathf.Sqrt(-gravity * jumpHeight * 1.25f);
                    break;
            }
        }

        private void GetInputs()
        {
            if (controls.autoRun.GetControlBindingDown())
                autoRun = !autoRun;

            //FORWARDS BACKWARDS CONTROLS  
            inputs.y = Axis(controls.forwards.GetControlBinding(), controls.backwards.GetControlBinding());

            if (inputs.y != 0 && !mainCam.autoRunReset)
                autoRun = false;

            if (autoRun)
            {
                inputs.y += Axis(true, false);

                inputs.y = Mathf.Clamp(inputs.y, -1, 1);
            }

            //STRAFE LEFT RIGHT
            inputs.x = Axis(controls.strafeRight.GetControlBinding(), controls.strafeLeft.GetControlBinding());

            if (steer)
            {
                inputs.x += Axis(controls.rotateRight.GetControlBinding(), controls.rotateLeft.GetControlBinding());

                inputs.x = Mathf.Clamp(inputs.x, -1, 1);
            }

            //ROTATE LEFT RIGHT
            if (steer)
                rotation = Input.GetAxis("Mouse X") * mainCam.CameraSpeed;
            else
                rotation = Axis(controls.rotateRight.GetControlBinding(), controls.rotateLeft.GetControlBinding());

            //ToggleRun
            if (controls.walkRun.GetControlBindingDown())
                run = !run;

            //Jumping
            jump = controls.jump.GetControlBinding();

            inputNormalized = inputs.normalized;
        }

        private void GetSwimDirection()
        {
            if (steer)
                swimDirection.eulerAngles = transform.eulerAngles + new Vector3(mainCam.tilt.eulerAngles.x, 0, 0);
        }

        private void Swimming()
        {
            if (!inWater && !jumping)
            {
                velocityY = 0;
                velocity = new Vector3(velocity.x, velocityY, velocity.z);
                jumpDirection = velocity;
                jumpSpeed = swimSpeed / 2;
                jumping = true;
                moveState = MoveState.locomotion;
            }

            //Rotating
            Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
            transform.eulerAngles = characterRotation;

            //setting ground ray
            _groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
            _groundRay.direction = Vector3.down;

            if (showGroundRay)
                Debug.DrawLine(_groundRay.origin, _groundRay.origin + Vector3.down * 0.15f, Color.red);

            if (!jumping && jump && d_fromWaterSurface <= swimLevel)
                Jump();

            if (!jumping)
            {
                velocity = swimDirection.forward * inputNormalized.y + swimDirection.right * inputNormalized.x;

                velocity.y += Axis(jump, controls.sit.GetControlBinding());

                velocity.y = Mathf.Clamp(velocity.y, -1, 1);

                velocity *= swimSpeed;

                _characterController.Move(velocity * Time.deltaTime);

                if (Physics.Raycast(_groundRay, out groundHit, 0.15f, groundMask))
                {
                    if (d_fromWaterSurface < swimLevel)
                        moveState = MoveState.locomotion;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, float.MinValue, waterSurface - swimLevel), transform.position.z);
                }
            }
            else
            {
                //Jump
                if (velocityY > terminalVelocity)
                    velocityY += gravity * Time.deltaTime;

                velocity = jumpDirection * jumpSpeed + Vector3.up * velocityY;

                _characterController.Move(velocity * Time.deltaTime);

                if (Physics.Raycast(_groundRay, out groundHit, 0.15f, groundMask))
                {
                    if (d_fromWaterSurface < swimLevel)
                        moveState = MoveState.locomotion;
                }

                if (d_fromWaterSurface >= swimLevel)
                    jumping = false;
            }
        }

        private void GetWaterlevel()
        {
            d_fromWaterSurface = waterSurface - transform.position.y;
            //d_fromWaterSurface = Mathf.Clamp(d_fromWaterSurface, 0, float.MaxValue);
        }

        public float Axis(bool pos, bool neg)
        {
            float axis = 0;

            if (pos)
                axis += 1;

            if (neg)
                axis -= 1;

            return axis;
        }

        private void DebugGroundNormals()
        {
            Vector3 lineStart = transform.position + Vector3.up * 0.05f;

            if (showMoveDirection)
                Debug.DrawLine(lineStart, lineStart + moveDirection.forward, Color.cyan);

            if (showForwardDirection)
                Debug.DrawLine(lineStart - groundDirection.forward * 0.5f, lineStart + groundDirection.forward * 0.5f, Color.blue);

            if (showStrafeDirection)
                Debug.DrawLine(lineStart - groundDirection.right * 0.5f, lineStart + groundDirection.right * 0.5f, Color.red);

            if (showFallNormal)
                Debug.DrawLine(lineStart, lineStart + fallDirection.up * 0.5f, Color.green);

            if (showSwimNormal)
                Debug.DrawLine(lineStart, lineStart + swimDirection.forward, Color.magenta);
        }

        private void OnControllerColliderHit(ControllerColliderHit controllerColliderHit)
        {
            if (controllerColliderHit.point.y <= transform.position.y + 0.25f)
            {
                if (_isGroundedBySphere)
                {
                    float gDistance = Vector3.Distance(IsGroundedSpherePosition, controllerColliderHit.point);

                    if (gDistance <= IsGroundedSphereRadius)
                    {
                        collisionPoint = controllerColliderHit.point;
                        collisionPoint = collisionPoint - transform.position;
                    }
                }
                else
                {
                    collisionPoint = controllerColliderHit.point;
                    collisionPoint = collisionPoint - transform.position;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (showGroundSphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(IsGroundedSpherePosition, IsGroundedSphereRadius);
            }
        }

        public enum MoveState { locomotion, swimming }
    }
}
