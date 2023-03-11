using System.Collections.Generic;
using Assets.Scripts.Abilities.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/AbilityTargetSO")]
    public class AbilityTargetSO : AbilitySO
    {
        [Header("AbilityTarget")]
        public bool IsLookOnTarget = true;
        public float Distance;
        public bool TargetIsAlwaysSelf = false;
        public EAbilityAffects CanCastOn = EAbilityAffects.Enemies;
        public List<GameObject> CollisionParticlePrefabs = new List<GameObject>();
        
        public override Ability CreateAbility(IAbilitiesController iAbilitiesController)
        {
            var ability = new AbilityTarget(this, iAbilitiesController);
            return ability;
        }
    }
}