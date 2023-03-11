using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.VFX.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.VFX
{
    public class SpawnEllipsePie : AbilityBehaviour
    {
        public SpawnEllipsePieSO SpawnEllipsePieSO => (SpawnEllipsePieSO) base.AbilityBehaviourSO;
        
        private static Object EllipsePiePrefab;
        private EllipsePie _ellipsePie;

        public float SpawnTime = 0.5f;
        public float Lifetime = 1.0f;
        public float FadeInTime = 0.5f;

        public Texture2D Texture;
        
        public SpawnEllipsePie(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            SpawnTime = SpawnEllipsePieSO.SpawnTime;
            Lifetime = SpawnEllipsePieSO.Lifetime;
            FadeInTime = SpawnEllipsePieSO.FadeInTime;
            Texture = SpawnEllipsePieSO.Texture;
        }

        // TODO Delete when boss fight resets.

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (!(ability is AbilityAOEFromSelf abilityAOEFromSelf))
            {
                Debug.LogError($"{nameof(SpawnEllipsePieSO)} can be used as behaviour only for ability \"{nameof(AbilityAOEFromSelf)}\".");
                return;
            }

            if (EllipsePiePrefab == null)
            {
                // Debug.Log($"Loading {nameof(EllipsePiePrefab)}");
                EllipsePiePrefab = Resources.Load(ConstantsResources.ELLIPSE_PIE);
            }

            SpawnGameObject(ability, iAbilityParameters, abilityAOEFromSelf);
        }
        
        private void SpawnGameObject(Ability ability, IAbilityParameters iAbilityParameters, AbilityAOEFromSelf abilityAOEFromSelf)
        {
            var source = iAbilityParameters.DefaultAbilityParameters.Source;
            var ellipsePieGO = (GameObject) MonoBehaviour.Instantiate(EllipsePiePrefab, source.GetRootObjectTransform().position, source.GetRootObjectTransform().rotation);

            _ellipsePie = ellipsePieGO.GetComponent<EllipsePie>();

            _ellipsePie.Height = abilityAOEFromSelf.Height;
            _ellipsePie.Radius = abilityAOEFromSelf.Radius;
            _ellipsePie.Angle = abilityAOEFromSelf.Angle;
            _ellipsePie.ClockwiseRotation = abilityAOEFromSelf.ClockwiseRotation;
            _ellipsePie.Texture = Texture;

            _ellipsePie.SpawnTime = SpawnTime;
            _ellipsePie.Lifetime = Lifetime;
            _ellipsePie.FadeInTime = FadeInTime;

            // _ellipsePie.Parent = source.GetRootObjectTransform();

            _ellipsePie.UpdateValues();
        }
    }
}