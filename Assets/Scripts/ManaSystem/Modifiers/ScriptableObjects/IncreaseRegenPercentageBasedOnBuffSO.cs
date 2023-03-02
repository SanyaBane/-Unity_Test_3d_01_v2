using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ManaModifier/IncreaseRegenPercentageBasedOnBuff")]
    public class IncreaseRegenPercentageBasedOnBuffSO : BaseManaModifierSO
    {
        public string BuffId;
        
        [Serializable]
        public class StacksIncreaseRegenPercentage
        {
            public int StacksCount;
            public float IncreaseRegenPercentage;
        }

        public List<StacksIncreaseRegenPercentage> ListStacksIncreaseRegenPercentage;

        public override BaseManaModifier CreateBaseManaModifier(IBaseCreature iBaseCreature)
        {
            var ret = new IncreaseRegenPercentageBasedOnBuff(this, iBaseCreature);
            return ret;
        }
    }
}