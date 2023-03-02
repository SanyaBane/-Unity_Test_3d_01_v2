using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class SpawnParticle : AbilityBehaviour
    {
        public SpawnParticleSO SpawnParticleSO => (SpawnParticleSO) base.AbilityBehaviourSO;
        
        public GameObject ParticlePrefab;
        
        public SpawnParticle(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            ParticlePrefab = SpawnParticleSO.ParticlePrefab;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var source = iAbilityParameters.DefaultAbilityParameters.Source;
            var particleGO = (GameObject) MonoBehaviour.Instantiate(ParticlePrefab, source.GetRootObjectTransform().position, source.GetRootObjectTransform().rotation);
            var particleSystem = particleGO.GetComponent<ParticleSystem>();
            // var particleController = particleGO.GetComponent<ParticleController>();

            // if (ability is AbilityAOEFromSelf abilityAOEFromSelf)
            // {
            //     particleController.SetRadius(abilityAOEFromSelf.Radius);
            // }

            particleSystem.Stop();

            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.loop = false;

            particleSystem.Play();
        }
    }
}