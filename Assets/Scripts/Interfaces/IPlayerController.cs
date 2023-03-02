using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPlayerController : IHasTransform
    {
        CharacterController CharacterController { get; }
        
        bool IsRunning { get; }

        bool IsTurningByMouse { get; }
        bool IsTurningByKeyboard { get; }

        float HorizontalInput { get; }
        float VerticalInput { get; }
        float StrafeInput { get; }
    }
}
