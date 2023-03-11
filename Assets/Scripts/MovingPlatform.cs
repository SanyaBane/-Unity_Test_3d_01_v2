using UnityEngine;
using Assets.Scripts.HelpersUnity;

namespace Assets.Scripts
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField]
        Transform platform;

        [SerializeField]
        Transform startTransform;

        [SerializeField]
        Transform endTransform;

        public float platformSpeed = 1.3f;
        public bool isMovingActive = false;

        public Vector3 currentDirection;
        public Transform currentDestinationTransform;

        void Start()
        {
            currentDirection = Vector3.zero;

            SetDestination(startTransform);
        }

        void FixedUpdate()
        {
            if (!isMovingActive)
                return;

            platform.GetComponent<Rigidbody>().MovePosition(platform.position + currentDirection * platformSpeed * Time.fixedDeltaTime);

            if (VectorHelper.DistanceSquared(platform.position, currentDestinationTransform.position) < Mathf.Pow(platformSpeed * Time.fixedDeltaTime, 2))
            {
                SetDestination(currentDestinationTransform == startTransform ? endTransform : startTransform);
            }
        }

        void SetDestination(Transform dest)
        {
            currentDestinationTransform = dest;
            currentDirection = (dest.position - platform.position).normalized;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(startTransform.position, platform.localScale);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(endTransform.position, platform.localScale);
        }

        void OnCollisionEnter()
        {
            Debug.Log("OnCollisionEnter()");
        }
    }
}