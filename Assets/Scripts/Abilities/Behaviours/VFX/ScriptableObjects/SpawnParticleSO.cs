using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.VFX.ScriptableObjects
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