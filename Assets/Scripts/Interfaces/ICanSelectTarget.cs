using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface ICanSelectTarget
    {
        ITargetable SelectedTarget { get; }

        event Action<ITargetable> SelectedTargetChanged;
    }
}

