using System.Collections.Generic;
using Assets.Scripts.Buffs.ScriptableObjects;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Applier
{
    public abstract class BaseBuffApplierSO : AbilityBehaviourSO
    {
        public List<BaseBuffSO> BuffsSO;
    }
}
