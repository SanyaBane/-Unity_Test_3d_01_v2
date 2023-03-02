//using Assets.Scripts.Abilities;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.Scripts.AutoAttack
//{
//    public class AutoAttackWithProjectile : AutoAttack
//    {
//        [Header("Projectile")]
//        public GameObject ProjectilePrefab;
//        public float ProjectileMoveSpeed = 20.0f;

//        protected override void ActuallyExecuteAutoAttack(ITargetable target)
//        {
//            HowLongDidntAutoAttack = 0;     //reset cooldown timer

//            if (ProjectilePrefab == null)
//                ProjectilePrefab = Resources.Load<GameObject>("Prefabs/Fallback/AutoAttackFallbackPrefab");

//            var autoAttackPrefabTransform = Instantiate(ProjectilePrefab, _attachmentsController.Attach_Attack1.position, Quaternion.identity);
//            var autoAttackPrefabScript = autoAttackPrefabTransform.GetComponent<RangeAutoAttackProjectile>();
//            autoAttackPrefabScript.OnReachTarget += AutoAttackPrefabScript_OnReachTarget;

//            autoAttackPrefabScript.Setup(target, ProjectileMoveSpeed, EProjectileMovementType.Straight);
//        }

//        private void AutoAttackPrefabScript_OnReachTarget(RangeAutoAttackProjectile rangeAutoAttackProjectile)
//        {
//            if (rangeAutoAttackProjectile.SelectedTarget.GetTransform() != null)
//            {
//                var targetHealth = rangeAutoAttackProjectile.SelectedTarget.GetTransform().GetComponent<Health>();
//                if (targetHealth != null)
//                {
//                    targetHealth.TakeDamage(GetAutoAttackDamage());
//                }
//            }

//            Destroy(rangeAutoAttackProjectile.gameObject);
//        }
//    }
//}
