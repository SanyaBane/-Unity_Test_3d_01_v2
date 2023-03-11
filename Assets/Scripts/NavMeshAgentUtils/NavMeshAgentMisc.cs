using Assets.Scripts.AutoAttack;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NavMeshAgentUtils
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentMisc : MonoBehaviour
    {
        public float Acceleration = 3f;
        public float Deceleration = 60f;

        public float RotationSpeed = 5f;

        private NavMeshAgent _navMeshAgent;
        private AutoAttackController _autoAttack;

        private void Start()
        {
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            _autoAttack = gameObject.GetComponent<AutoAttackController>();

            //Acceleration = _navMeshAgent.acceleration;
        }

        private void Update()
        {
            //if (_navMeshAgent != null)
            //{
            //    // speed up slowly, but stop quickly
            //    if (_navMeshAgent.hasPath)
            //    {
            //        float minDistanceToTarget = _autoAttack.AutoAttackPropertiesSO.MaxDistance * 0.5f; // Distance where Creature should immediately be stopped.

            //        if (_navMeshAgent.remainingDistance < minDistanceToTarget)
            //        {
            //            _navMeshAgent.acceleration = Deceleration;
            //        }

            //        //_navMeshAgent.acceleration = (_navMeshAgent.remainingDistance < minDistanceToTarget) ? Deceleration : Acceleration;
            //    }
            //}
        }
    }
}
