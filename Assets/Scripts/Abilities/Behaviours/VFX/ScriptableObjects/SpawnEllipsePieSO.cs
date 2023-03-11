using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.VFX.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/VFX/SpawnEllipsePieSO")]
    public class SpawnEllipsePieSO : AbilityBehaviourSO
    {
        private static Object EllipsePiePrefab;
        private EllipsePie _ellipsePie;

        public float SpawnTime = 0.5f;
        public float Lifetime = 1.0f;
        public float FadeInTime = 0.5f;

        [Header("General")]
        public Texture2D Texture;

        // TODO Delete when boss fight resets.

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new SpawnEllipsePie(this);
            return ret;
        }
    }
}