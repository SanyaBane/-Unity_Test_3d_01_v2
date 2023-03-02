using System;
using System.Collections.Generic;
using Assets.Scripts.Factions;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using UnityEngine;

namespace Assets.Scripts.Abilities.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/AbilityAOEFromSelfSO")]
    public class AbilityAOEFromSelfSO : AbilitySO
    {
        [Header("AbilityAOEFromSelfSO")]
        public bool DisplayAOEZone = false;

        public EAbilityAffects AbilityAffects = EAbilityAffects.Enemies;

        public float Height = 6.0f;
        public float Radius = 5.0f;

        [Range(0, 360)]
        public float Angle = 90;

        [Range(0, 360)]
        public float ClockwiseRotation = 0;

        [Header("VFX")]
        public List<GameObject> ParticlesCasterCastFinish = new List<GameObject>();
        
        public override Ability CreateAbility(IAbilitiesController iAbilitiesController)
        {
            var ability = new AbilityAOEFromSelf(this, iAbilitiesController);
            return ability;
        }
    }
}