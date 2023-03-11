using System;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/OnCreateCustomLogic/ChangeManaCostBasedOnBuff")]
    public class ChangeManaCostBasedOnBuffSO : BaseOnCreateCustomLogicSO
    {
        public string BuffId;

        [Serializable]
        public class StacksToManaCost
        {
            public int StacksCount;
            public float ManaCostPercentage;
        }

        public StacksToManaCost[] ListStacksToManaCost;

        public override BaseOnCreateCustomLogic CreateOnCreateCustomLogicObject(IAbilitiesController iAbilitiesController, Ability ability)
        {
            var ret = new ChangeManaCostBasedOnBuff(this, iAbilitiesController, ability);
            return ret;
        }
    }
}