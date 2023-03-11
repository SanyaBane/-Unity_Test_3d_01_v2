using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.ScriptableObjects
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
