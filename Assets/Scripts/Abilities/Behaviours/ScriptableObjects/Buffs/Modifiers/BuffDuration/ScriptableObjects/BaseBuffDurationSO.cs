using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects
{
    public abstract class BaseBuffDurationSO : ScriptableObject
    {
        public float InitialDuration = 9;

        public bool IsPermanent = false;

        public abstract BaseBuffDuration CreateBaseBuffDuration();
    }
}