using UnityEngine;

namespace Assets.Scripts.TestScripts
{
    public class TestRotateGraphAroundYAxis : MonoBehaviour
    {
        public bool DoRotation = true;
        public float Speed = 10.0f;

        void Update()
        {
            if (DoRotation)
            {
                Vector3 rotation = this.transform.rotation.eulerAngles;
                rotation.y += Time.deltaTime * Speed;

                this.transform.rotation = Quaternion.Euler(rotation);
            }
            
            // this.transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }
}