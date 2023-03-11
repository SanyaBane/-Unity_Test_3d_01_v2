using UnityEngine;

namespace Assets.Scripts.PlayerController
{
    public class GroundedInfo : MonoBehaviour
    {
        [SerializeField] private bool _isGroundedByRaycast = true;

        public bool IsGroundedByRaycast
        {
            get => _isGroundedByRaycast;
            set => _isGroundedByRaycast = value;
        }

        [SerializeField] private bool _isGroundedByCharController = true;

        public bool IsGroundedByCharController
        {
            get => _isGroundedByCharController;
            private set => _isGroundedByCharController = value;
        }

        [SerializeField] private bool _isGroundedBySphere = false;

        public bool IsGroundedBySphere
        {
            get => _isGroundedBySphere;
            private set { _isGroundedBySphere = value; }
        }

        [SerializeField] private float _howLongGrounded = 0;
        public float HowLongGrounded => _howLongGrounded;
        [SerializeField] private float _howLongNotGrounded = 0;
        public float HowLongNotGrounded => _howLongNotGrounded;

        [SerializeField] private bool _slidingDownTheSlope = false;
        public bool SlidingDownTheSlope
        {
            get => _slidingDownTheSlope;
            set => _slidingDownTheSlope = value;
        }

        [SerializeField] private float _isGroundedSphereRadius;
        public float IsGroundedSphereRadius
        {
            get => _isGroundedSphereRadius;
            private set => _isGroundedSphereRadius = value;
        }
        //public float _isGroundedSphereRadius => _characterController.radius;

        [SerializeField] private Vector3 _isGroundedSpherePosition;
        public Vector3 IsGroundedSpherePosition
        {
            get => _isGroundedSpherePosition;
            private set => _isGroundedSpherePosition = value;
        }
        //public Vector3 _isGroundedSpherePosition
        //{
        //    get
        //    {
        //        return _playerCreature.GetTransform().position // спавним прямо под персонажем
        //            + new Vector3(0, _isGroundedSphereRadius, 0) // сдвигаем наверх на значение равное радиусу
        //            + new Vector3(0, -IsGroundedSphereOffsetY, 0); // делаем сдвиг вниз чтобы сфера была чуть ниже Character Controller'a
        //    }
        //}

        //public float IsGroundedSphereOffsetY = 0.01f;
        public LayerMask GroundMask;

        [SerializeField] private float _stepOffsetCCWhenGrounded = 0.3f;
        [SerializeField] private float _stepOffsetCCWhenNotGrounded = 0.01f;

        private CharacterController _characterController;
        
        protected void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }
        
        public void UpdateIsGrounded()
        {
            GroundSphereCheck();

            if (IsGroundedByCharController && _slidingDownTheSlope == false)
            {
                if (_howLongGrounded < 60000.0f) // to prevent overflow?
                    _howLongGrounded += Time.deltaTime;

                _howLongNotGrounded = 0;
            }
            else
            {
                if (_howLongNotGrounded < 60000.0f) // to prevent overflow?
                    _howLongNotGrounded += Time.deltaTime;

                _howLongGrounded = 0;
            }

            //bool checkBySphere = Physics.CheckSphere(IsGroundedSpherePosition, _isGroundedSphereRadius, GroundMask);
            //IsGrounded = checkBySphere;
            IsGroundedByCharController = _characterController.isGrounded;


            // когда персонаж на земле, пусть у него будет возможность забираться на относительно высокие препятствия, 
            // но когда персонаж в воздухе (поднимается или падает), stepOffset надо серьёзно ограничивать
            _characterController.stepOffset = IsGroundedByCharController ? _stepOffsetCCWhenGrounded : _stepOffsetCCWhenNotGrounded;
        }
        
        private void GroundSphereCheck()
        {
            float gSkin = _characterController.skinWidth * 1.2f;

            IsGroundedSphereRadius = Mathf.Cos(_characterController.slopeLimit * Mathf.Deg2Rad);
            IsGroundedSphereRadius = IsGroundedSphereRadius / (1 - IsGroundedSphereRadius);
            IsGroundedSphereRadius *= gSkin;
            IsGroundedSphereRadius = _characterController.radius - IsGroundedSphereRadius;

            IsGroundedSpherePosition = transform.position + Vector3.up * (IsGroundedSphereRadius - gSkin);
            //Debug.Log($"IsGroundedSpherePosition (local): {IsGroundedSpherePosition - _playerCreature.GetTransform().position}");

            IsGroundedBySphere = Physics.CheckSphere(IsGroundedSpherePosition, IsGroundedSphereRadius, GroundMask);
        }
    }
}