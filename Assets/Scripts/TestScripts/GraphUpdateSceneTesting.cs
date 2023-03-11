using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.TestScripts
{
    public class GraphUpdateSceneTesting : MonoBehaviour
    {
        private GraphUpdateScene _graphUpdateScene;
        private Collider _collider;

        private void Awake()
        {
            _graphUpdateScene = this.GetComponent<GraphUpdateScene>();
            _collider = this.GetComponent<Collider>();
        }

        private float nextActionTime = 0.0f;
        public float period = 0.5f;

        private void Update()
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                // execute block of code here

                var guo = new GraphUpdateObject();
                guo.bounds = _collider.bounds;
                AstarPath.active.UpdateGraphs(guo);
                
                // _graphUpdateScene.RecalcConvex();
                // _graphUpdateScene.set
                _graphUpdateScene.Apply();
            }
        }
    }
}