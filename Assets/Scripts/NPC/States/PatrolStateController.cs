using System;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    [RequireComponent(typeof(NpcAI))]
    public class PatrolStateController : MonoBehaviour
    {
        private NpcAI _NpcAI;
        
        [Header("Patroling")]
        public Transform[] Destinations;
        public int DestinationCurrentIndex;
        public Vector3? PatrolCurrentDestination;

        public float PatrolReachedDestinationDistanceCheck = 0.5f;
        public float BreakWhilePatroling = 1.0f;

        public bool CanPatrol = true;
        public bool IsInPatrolingMode = false;

        private void Awake()
        {
            _NpcAI = GetComponent<NpcAI>();
        }
        
        public Func<bool> PatrolingReachedDestination() => () =>
        {
            if (Destinations.Length == 0)
                return false;

            // var distance = Vector3.Distance(_NpcAI.INpcBaseCreature.GetTransform().position, Destinations[DestinationCurrentIndex].position);
            var distanceSquared = (_NpcAI.INpcBaseCreature.GetRootObjectTransform().position - Destinations[DestinationCurrentIndex].position).sqrMagnitude;
            var ret = distanceSquared < (PatrolReachedDestinationDistanceCheck * PatrolReachedDestinationDistanceCheck);
            return ret;
        };
    }
}