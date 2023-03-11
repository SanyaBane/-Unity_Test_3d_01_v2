using Assets.Scripts.Abilities.General;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects
{
    public abstract class BaseOnCreateCustomLogicSO : ScriptableObject
    {
        public abstract BaseOnCreateCustomLogic CreateOnCreateCustomLogicObject(IAbilitiesController iAbilitiesController, Ability ability);

        protected virtual void OnEnable()
        {
        }
    }
}