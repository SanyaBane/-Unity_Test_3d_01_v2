using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Scripts.AreaOfEffects;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts
{
    public class AOEManager : MonoBehaviour
    {
        private List<AOECircleController> AoeCircleControllers = new List<AOECircleController>();

        public void AddNewElementToAORCircleControllers(AOECircleController aoeCircleController)
        {
            AoeCircleControllers.Add(aoeCircleController);
        }

        public IEnumerable<AOECircleController> GetAOECircleControllers()
        {
            foreach (var aoeCircleController in AoeCircleControllers)
            {
                yield return aoeCircleController;
            }
        }

        public IEnumerable<AOECircleController> GetActiveAOECircleControllers()
        {
            foreach (var aoeCircleController in AoeCircleControllers)
            {
                if (aoeCircleController.gameObject.activeInHierarchy == false)
                    continue;

                yield return aoeCircleController;
            }
        }

        private float nextActionTime = 0.0f;
        public float period = 0.5f;

        [Header("Debug")]
        public bool DisplayNodesWithPenalty = true;

        public float SizeNodesWithPenalty = 0.1f;

        private void Start()
        {
            nextActionTime = Time.time;
        }

        private object _graphNodesForTestingLocker = new Object();
        public List<GraphNode> GraphNodesForTesting = new List<GraphNode>();

        public void AddGraphNodeForTesting(GraphNode graphNode)
        {
            lock (_graphNodesForTestingLocker)
            {
                GraphNodesForTesting.Add(graphNode);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (DisplayNodesWithPenalty)
            {
                lock (_graphNodesForTestingLocker)
                {
                    foreach (var graphNodeForTesting in GraphNodesForTesting)
                    {
                        switch (graphNodeForTesting.Tag)
                        {
                            case 0:
                                continue;
                            case AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_1:
                                // Gizmos.color = Color.green;
                                Gizmos.color = new Color(1, 0.8f, 0.8f, 1);
                                break;
                            case AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_2:
                                // Gizmos.color = Color.blue;
                                Gizmos.color = new Color(1, 0.45f, 0.45f, 1);
                                break;
                            case AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_3:
                                Gizmos.color = new Color(1, 0, 0, 1);
                                break;
                        }

                        Gizmos.DrawSphere((Vector3) graphNodeForTesting.position, SizeNodesWithPenalty);
                    }
                }
            }
        }

        private void Update()
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                lock (_graphNodesForTestingLocker)
                {
                    GraphNodesForTesting.Clear();
                }

                foreach (var aoeCircleController in AoeCircleControllers)
                {
                    aoeCircleController.UpdateTagsInfo();
                }
            }
        }
    }
}