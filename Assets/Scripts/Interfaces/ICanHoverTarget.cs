using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface ICanHoverTarget
    {
        ITargetable HoveredTarget { get; }

        event Action<ITargetable> HoveredTargetChanged;
    }
}
