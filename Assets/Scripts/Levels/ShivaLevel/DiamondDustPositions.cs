using UnityEngine;

namespace Assets.Scripts.Levels
{
    public class DiamondDustPositions : MonoBehaviour
    {
        [Header("General")]
        public Transform StarPuddleNWTransform;
        public Transform StarPuddleNETransform;
        public Transform StarPuddleSETransform;
        public Transform StarPuddleSWTransform;
        
        public Transform BeforeKnockBackTankMeleeNorthTransform;
        public Transform BeforeKnockBackTankMeleeWestTransform;
        public Transform BeforeKnockBackHealRangeEastTransform;
        public Transform BeforeKnockBackHealRangeSouthTransform;
    }
}