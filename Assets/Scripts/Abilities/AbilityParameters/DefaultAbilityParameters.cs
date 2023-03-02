using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class DefaultAbilityParameters
    {
        public IBaseCreature Source { get; }
        public ITargetable Target { get; }

        public bool IsTargetSameAsSource => Target != null && Target.IBaseCreature == Source;

        public DefaultAbilityParameters(IBaseCreature source, ITargetable target)
        {
            Source = source;
            Target = target;
        }
    }
}