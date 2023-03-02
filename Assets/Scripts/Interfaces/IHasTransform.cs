using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IHasTransform
    {
        Transform GetRootObjectTransform();
        Vector3 GetGroundedPosition();
    }
}
