using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects
{
    public abstract class BaseBuffRecastTypeSO : ScriptableObject
    {
        public abstract BaseBuffRecastType CreateBaseBuffRecastType();
    }
}