using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/AbilityTargetProjectileSO")]
    public class AbilityTargetProjectileSO : AbilityTargetSO
    {
        [Header("AbilityTargetProjectileSO")]

        [Header("Projectile")]
        public GameObject PrefabGameObject;
        public float MoveSpeed = 15;

        [Header("Projectile Movement")]
        public EAbilityProjectileMovementType abilityProjectileMovementType;
        //public float RotatingAngle = 1.0f;
        
        public override Ability CreateAbility(IAbilitiesController iAbilitiesController)
        {
            var ability = new AbilityTargetProjectile(this, iAbilitiesController);
            return ability;
        }
    }
}
