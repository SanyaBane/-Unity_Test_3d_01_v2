using UnityEngine;

namespace Assets.Scripts
{
    public class SelfDestruct : MonoBehaviour
    {

        public float SelfDestructTime = 1.0f;

        [SerializeField]
        private float timePassed = 0;

        void Update()
        {
            timePassed += Time.deltaTime;

            if (timePassed >= SelfDestructTime)
            {
                Destroy(gameObject);
            }

        }
    }
}
