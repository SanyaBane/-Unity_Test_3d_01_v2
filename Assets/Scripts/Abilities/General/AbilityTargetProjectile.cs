using Assets.Scripts.Abilities.Enums;
using Assets.Scripts.Abilities.General.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Abilities.Specific;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.General
{
    public class AbilityTargetProjectile : AbilityTarget
    {
        public AbilityTargetProjectileSO AbilityTargetDamageProjectileSO => (AbilityTargetProjectileSO) AbilitySO;

        public GameObject PrefabGameObject;
        public float MoveSpeed;
        public EAbilityProjectileMovementType AbilityProjectileMovementType;

        //public float RotatingAngle = 1.0f;
        private AbilityRangeProjectile _abilityRangeProjectile;

        public AbilityTargetProjectile(AbilityTargetProjectileSO abilityTargetProjectileSO, IAbilitiesController iAbilitiesController) : base(abilityTargetProjectileSO, iAbilitiesController)
        {
            PrefabGameObject = abilityTargetProjectileSO.PrefabGameObject;
            MoveSpeed = abilityTargetProjectileSO.MoveSpeed;
            AbilityProjectileMovementType = abilityTargetProjectileSO.abilityProjectileMovementType;
        }

        public override void ExecuteAbility(IAbilityParameters iAbilityParameters)
        {
            var autoAttackPrefabTransform = MonoBehaviour.Instantiate(PrefabGameObject, iAbilityParameters.DefaultAbilityParameters.Source.AttachmentsController.Attach_Attack1.position, Quaternion.identity);

            _abilityRangeProjectile = autoAttackPrefabTransform.GetComponent<AbilityRangeProjectile>();
            _abilityRangeProjectile.OnReachTarget += AutoAttackPrefabScript_OnReachTarget;

            _abilityRangeProjectile.Setup(this, iAbilityParameters, MoveSpeed, AbilityProjectileMovementType);
        }

        public override bool IsAbilityCanStartCast(IAbilityParameters abilityParameters, bool displayErrorMessage)
        {
            if (base.IsAbilityCanStartCast(abilityParameters, displayErrorMessage) == false)
                return false;

            if (abilityParameters.DefaultAbilityParameters.Target.CanBeAttacked == false)
                return false;

            return true;
        }

        protected void AutoAttackPrefabScript_OnReachTarget(Ability ability, AbilityRangeProjectile rangeAutoAttackProjectile)
        {
            if (rangeAutoAttackProjectile.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                rangeAutoAttackProjectile.Target.ProjectilesOnTheWay.Remove(_abilityRangeProjectile);

                ApplyAbilityBehaviours(rangeAutoAttackProjectile.IAbilityParameters);
            }

            foreach (var element in CollisionParticlePrefabs)
            {
                MonoBehaviour.Instantiate(element, rangeAutoAttackProjectile.transform.position, Quaternion.identity);
            }

            MonoBehaviour.Destroy(rangeAutoAttackProjectile.gameObject);
        }
    }
}