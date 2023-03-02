using System;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember
{
    public static class PartyMemberHelper
    {
        public static Vector3 GetBattlePosition(ERole role, Vector3 approximatePosition, Vector3 offsetPositionRotation)
        {
            var offsetBetweenBuddies = 0.3f;
            var mtPosition = approximatePosition; // + (offsetPositionRotation);
            switch (role)
            {
                case ERole.None:
                case ERole.MT:
                    return mtPosition;
                case ERole.OT:
                    return mtPosition + (offsetPositionRotation * offsetBetweenBuddies);
                case ERole.H1:
                    return approximatePosition;
                case ERole.H2:
                    return approximatePosition;
                case ERole.M1:
                    return approximatePosition + new Vector3(-offsetBetweenBuddies, 0, 0);
                    // return approximatePosition + new Vector3(0, 0, 0);
                case ERole.M2:
                    return approximatePosition + new Vector3(offsetBetweenBuddies, 0, 0);
                    // return approximatePosition + new Vector3(0, 0, 0);
                case ERole.R1:
                    return approximatePosition;
                case ERole.R2:
                    return approximatePosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }
    }
}