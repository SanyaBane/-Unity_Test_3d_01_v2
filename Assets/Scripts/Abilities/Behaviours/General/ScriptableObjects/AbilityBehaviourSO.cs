using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects
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
