using System;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    public abstract class AbilityBehaviourSO : ScriptableObject
    {
        public bool WorksOnlyIfComboPerformed = false;
        
        public abstract AbilityBehaviour CreateAbilityBehaviour();

        protected virtual void OnEnable()
        {
        }
    }
}
