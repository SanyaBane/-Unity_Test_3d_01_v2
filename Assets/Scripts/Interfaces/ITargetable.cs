using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities;

namespace Assets.Scripts.Interfaces
{
    public interface ITargetable
    {
        IBaseCreature IBaseCreature { get; }
        BaseHealth Health { get; }

        bool CanBeTargeted { get; set; }
        event Action<bool> CanBeTargetedChanged;

        bool CanBeAttacked { get; set; }
        event Action<bool> CanBeAttackedChanged;

        string NameWhenTargeted { get; }
        bool HasFrontAndBack { get; }
        bool CatchAOEByCapsuleCollider { get; }

        float AutoAttackIndicatorHeight { get; }
        // Transform IndicatorBone { get; }

        List<AbilityRangeProjectile> ProjectilesOnTheWay { get; }
    }
}
