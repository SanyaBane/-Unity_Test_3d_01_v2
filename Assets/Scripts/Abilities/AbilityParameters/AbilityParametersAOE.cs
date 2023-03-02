using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class AbilityParametersAOE : AbilityParameters//, IAbilityParametersAOE
    {
        public List<IBaseCreature> Targets { get; set; }
    }
}
