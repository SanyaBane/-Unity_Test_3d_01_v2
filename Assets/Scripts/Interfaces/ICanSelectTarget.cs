using System;

namespace Assets.Scripts.Interfaces
{
    public interface ICanSelectTarget
    {
        ITargetable SelectedTarget { get; }

        event Action<ITargetable> SelectedTargetChanged;
    }
}

