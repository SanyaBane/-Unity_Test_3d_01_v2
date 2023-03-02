using Assets.Scripts.Factions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [CreateAssetMenu(menuName = "Creatures/CreatureSO")]
    public class CreatureSO : ScriptableObject
    {
        public string Name = "Creature_Name";
        public bool HasFrontAndBack = true;
        public bool CatchAOEByCapsuleCollider = true;

        public Faction DefaultFaction;
        public EJob Job;
        public ERace Race;
    }
}
