using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/Duration/BuffStackDuration")]
    public class BuffStackDurationSO : BuffDurationDefaultSO
    {
        public float MaxDuration = 9;

        public override BaseBuffDuration CreateBaseBuffDuration()
        {
            var ret = new BuffStackDuration(this);
            return ret;
        }
    }
}
