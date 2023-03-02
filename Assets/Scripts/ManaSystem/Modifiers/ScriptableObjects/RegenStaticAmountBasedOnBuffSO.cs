using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ManaModifier/RegenStaticAmountBasedOnBuff")]
    public class RegenStaticAmountBasedOnBuffSO : BaseManaModifierSO
    {
        public string BuffId;
        
        [Serializable]
        public class StacksRegenStaticAmount
        {
            public int StacksCount;
            public int Amount;
        }

        public List<StacksRegenStaticAmount> ListStacksRegenStatic;

        public override BaseManaModifier CreateBaseManaModifier(IBaseCreature iBaseCreature)
        {
            var ret = new RegenStaticAmountBasedOnBuff(this, iBaseCreature);
            return ret;
        }
    }
}