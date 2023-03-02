using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs
{
    public abstract class BaseBuffApplierSO : AbilityBehaviourSO
    {
        public List<BaseBuffSO> BuffsSO;
    }
}
