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
