using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class AbilityParameters : IAbilityParameters
    {
        public DefaultAbilityParameters DefaultAbilityParameters { get; set; }
    }
}
