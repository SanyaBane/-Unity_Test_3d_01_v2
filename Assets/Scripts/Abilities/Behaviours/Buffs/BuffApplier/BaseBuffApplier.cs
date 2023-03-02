using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public abstract class BaseBuffApplier : AbilityBehaviour
    {
        public BaseBuffApplierSO BaseBuffApplierSO => (BaseBuffApplierSO) base.AbilityBehaviourSO;
        
        public List<BaseBuffSO> BuffsSO;
        
        protected BaseBuffApplier(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            BuffsSO = BaseBuffApplierSO.BuffsSO;
        }
    }
}