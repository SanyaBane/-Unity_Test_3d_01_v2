using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
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