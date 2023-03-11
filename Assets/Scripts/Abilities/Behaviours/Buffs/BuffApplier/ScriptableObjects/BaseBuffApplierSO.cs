using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Buffs.ScriptableObjects;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects
{
    public abstract class BaseBuffApplierSO : AbilityBehaviourSO
    {
        public List<BaseBuffSO> BuffsSO;
    }
}
