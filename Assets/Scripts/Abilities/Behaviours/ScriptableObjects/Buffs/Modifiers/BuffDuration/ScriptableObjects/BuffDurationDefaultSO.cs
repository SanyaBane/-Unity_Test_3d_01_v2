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
