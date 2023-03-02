using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Creatures;
using Assets.Scripts.HelpersUnity;
using UnityEngine;
using Random = UnityEngine.Random;

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