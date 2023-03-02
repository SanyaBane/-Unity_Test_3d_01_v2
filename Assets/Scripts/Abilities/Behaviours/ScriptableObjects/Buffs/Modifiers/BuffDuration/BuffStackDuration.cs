using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration
{
    public class BuffStackDuration : BuffDurationDefault
    {
        private BuffStackDurationSO _buffStackDurationSO;

        public float MaxDuration;

        public BuffStackDuration(BaseBuffDurationSO baseBuffDurationSO) : base(baseBuffDurationSO)
        {
            _buffStackDurationSO = (BuffStackDurationSO) baseBuffDurationSO;

            MaxDuration = _buffStackDurationSO.MaxDuration;
        }

        public override void UpdateDuration(Buff buff)
        {
            RemainingDuration += InitialDuration;

            if (RemainingDuration > MaxDuration)
                RemainingDuration = MaxDuration;

            RemainingDurationOnLastUpdateDuration = RemainingDuration;
        }
    }
}