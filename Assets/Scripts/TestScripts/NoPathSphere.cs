using UnityEngine;

namespace Assets.Scripts.TestScripts
{
    [RequireComponent(typeof(SphereCollider))]
    public class NoPathSphere : MonoBehaviour
    {
        private SphereCollider _sphereCollider;

        public uint ModifyTag = 2;

        private float nextActionTime = 0.0f;
        public float period = 0.5f;

        private void Start()
        {
            _sphereCollider = this.GetComponent<SphereCollider>();
            nextActionTime = Time.time;
        }

        private void Update()
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
            
                // var guo = new NoPathSphereGraphUpdateObject(_sphereCollider.radius);
                // guo.bounds = GetComponent<SphereCollider>().bounds;
                // guo.updatePhysics = false;
                // guo.modifyTag = true;
                // guo.setTag = guo.setTag | (int) ModifyTag;
                //
                // // guo.RevertFromBackup();
                //
                // AstarPath.active.UpdateGraphs(guo);
            }
        }
    }
}