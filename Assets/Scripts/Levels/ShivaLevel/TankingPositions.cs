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
    public class TankingPositions : MonoBehaviour
    {
        [Header("General")]
        public Transform PoolPosition;
        public Transform AfterPoolPosition;
    }
}