using System;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/OnCreateCustomLogic/ChangeCastSpeedBasedOnBuff")]
    public class ChangeCastSpeedBasedOnBuffSO : BaseOnCreateCustomLogicSO
    {
        // public string ModifierId;
        public string BuffId;

        [Serializable]
        public class StacksToCastSpeedPercentage
        {
            public int StacksCount;
            public float CastSpeedPercentage;
        }

        public StacksToCastSpeedPercentage[] ListStacksToCastSpeedPercentage;

        public override BaseOnCreateCustomLogic CreateOnCreateCustomLogicObject(IAbilitiesController iAbilitiesController, Ability ability)
        {
            var ret = new ChangeCastSpeedBasedOnBuff(this, iAbilitiesController, ability);
            return ret;
        }

        protected override void OnEnable()
        {
            // if (String.IsNullOrEmpty(ModifierId))
            //     Debug.LogError($"{nameof(ModifierId)} == null in {nameof(ChangeCastSpeedBasedOnBuffSO)}");
        }
    }
}