using Assets.Scripts.Interfaces;

namespace Assets.Scripts.UI
{
    public class PlayerFrameTargetInfo
    {
        public ITargetable PlayerTarget { get; }
        public bool IsSubscribedToCombatInfo { get; set; }
        public CombatInfo CombatInfo { get; set; }

        public PlayerFrameTargetInfo(ITargetable target)
        {
            PlayerTarget = target;
            IsSubscribedToCombatInfo = false;
        }
    }
}