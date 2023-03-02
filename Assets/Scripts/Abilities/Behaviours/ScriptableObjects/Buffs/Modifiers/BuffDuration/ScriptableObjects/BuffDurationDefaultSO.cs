using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/Duration/Default")]
    public class BuffDurationDefaultSO : BaseBuffDurationSO
    {
        public override BaseBuffDuration CreateBaseBuffDuration()
        {
            var ret = new BuffDurationDefault(this);
            return ret;
        }
    }
}
