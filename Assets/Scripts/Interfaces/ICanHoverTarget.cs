using System;

namespace Assets.Scripts.Interfaces
{
    public interface ICanHoverTarget
    {
        ITargetable HoveredTarget { get; }

        event Action<ITargetable> HoveredTargetChanged;
    }
}
