using Assets.Scripts.Interfaces;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/VFX/SpawnParticleSO")]
    public class SpawnParticleSO : AbilityBehaviourSO
    {
        public GameObject ParticlePrefab;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new SpawnParticle(this);
            return ret;
        }
    }
}