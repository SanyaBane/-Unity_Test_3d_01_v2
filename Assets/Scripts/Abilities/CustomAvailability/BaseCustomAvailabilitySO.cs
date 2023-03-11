using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Abilities.CustomAvailability
{
    public class BaseCustomAvailabilitySO : ScriptableObject
    {
        public virtual bool IsCanStartCast(IAbilityParameters iAbilityParameters)
        {
            return true;
        }
    }
}