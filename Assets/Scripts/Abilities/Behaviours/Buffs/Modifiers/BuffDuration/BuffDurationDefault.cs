using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.ScriptableObjects;
using Assets.Scripts.Buffs;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration
{
    public class BuffDurationDefault : BaseBuffDuration
    {
        private BuffDurationDefaultSO _buffDurationDefaultSO;

        public BuffDurationDefault(BaseBuffDurationSO baseBuffDurationSO) : base(baseBuffDurationSO)
        {
            _buffDurationDefaultSO = (BuffDurationDefaultSO) baseBuffDurationSO;
        }

        public override void TickDuration(Buff buff)
        {
            if (IsPermanent)
                return;

            RemainingDuration -= Time.deltaTime;

            if (RemainingDuration <= 0)
            {
                buff.SetBuffReadyToBeRemoved();
            }
        }

        public override void UpdateDuration(Buff buff)
        {
            RemainingDuration = InitialDuration;
            RemainingDurationOnLastUpdateDuration = RemainingDuration;
        }
    }
}