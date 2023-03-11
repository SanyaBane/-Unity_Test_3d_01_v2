using Assets.Scripts.Enums;
using Assets.Scripts.Factions;
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
