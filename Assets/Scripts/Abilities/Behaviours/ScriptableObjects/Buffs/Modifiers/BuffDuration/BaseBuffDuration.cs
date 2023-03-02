using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects;
using Assets.Scripts.Buffs;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration
{
    public abstract class BaseBuffDuration
    {
        public BaseBuffDurationSO BaseBuffDurationSO { get; }

        public float InitialDuration { get; }
        public bool IsPermanent { get; }

        public float RemainingDuration { get; protected set; }
        public float RemainingDurationOnLastUpdateDuration { get; protected set; }
        
        public BaseBuffDuration(BaseBuffDurationSO baseBuffDurationSO)
        {
            BaseBuffDurationSO = baseBuffDurationSO;
            
            InitialDuration = BaseBuffDurationSO.InitialDuration;
            IsPermanent = BaseBuffDurationSO.IsPermanent;
            
            RemainingDuration = InitialDuration;
            RemainingDurationOnLastUpdateDuration = RemainingDuration;
        }
        
        public abstract void TickDuration(Buff buff);
        public abstract void UpdateDuration(Buff buff);
    }
}