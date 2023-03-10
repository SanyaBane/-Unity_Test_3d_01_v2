using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/RecastType/DefaultUpdateDuration")]
    public class BuffRecastTypeDefaultUpdateDurationSO : BaseBuffRecastTypeSO
    {
        public override BaseBuffRecastType CreateBaseBuffRecastType()
        {
            var ret = CreateBuffRecastTypeDefaultRestart();
            return ret;
        }

        public static BuffRecastTypeDefaultUpdateDuration CreateBuffRecastTypeDefaultRestart()
        {
            var ret = new BuffRecastTypeDefaultUpdateDuration(null);
            return ret;
        }
    }
}