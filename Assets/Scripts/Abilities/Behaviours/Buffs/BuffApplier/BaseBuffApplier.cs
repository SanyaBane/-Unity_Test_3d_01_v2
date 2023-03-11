using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Applier;
using Assets.Scripts.Buffs.ScriptableObjects;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier
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