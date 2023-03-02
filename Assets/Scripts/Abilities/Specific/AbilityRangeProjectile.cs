using Assets.Scripts.Interfaces;
using System;
using Assets.Scripts.HelpersUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class AbilityRangeProjectile : MonoBehaviour//, ICanSelectTarget
    {
        public AbilityTargetProjectile AbilityTargetProjectile { get; private set; }
        public IAbilityParameters IAbilityParameters { get; private set; }

        public IBaseCreature Source => IAbilityParameters.DefaultAbilityParameters.Source;
        public ITargetable Target => IAbilityParameters.DefaultAbilityParameters.Target;
        
        private bool _isInitialized = false;
        public event Action<Ability, AbilityRangeProjectile> OnReachTarget;

        private IHaveHitloc _targetHitloc;
        private float _moveSpeed = 0;
        private bool _lostTargetOnce = false;
        private Vector3 _lastKnownTargetPosition;

        public void Setup(AbilityTargetProjectile abilityTargetProjectile, IAbilityParameters iAbilityParameters, float moveSpeed, EAbilityProjectileMovementType abilityProjectileMovementType)
        {
            AbilityTargetProjectile = abilityTargetProjectile;
            IAbilityParameters = iAbilityParameters;

            _moveSpeed = moveSpeed;
            _lastKnownTargetPosition = IAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform().position;

            _targetHitloc = IAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature;
            if (_targetHitloc == null)
                Debug.LogError($"{nameof(_targetHitloc)} == null");

            if (abilityProjectileMovementType == EAbilityProjectileMovementType.Rotating)
            {
                var rotateProjectileChild = this.GetComponent<RotateProjectileChild>();
                rotateProjectileChild.Setup(_targetHitloc.GetHitloc());
            }

            Target.ProjectilesOnTheWay.Add(this);

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            
            var speedWithAppliedTime = _moveSpeed * Time.deltaTime;

            // if target was at least once null, then set flag "_lostTargetOnce" to true
            if (Target.IBaseCreature.GetRootObjectTransform() == null)
                _lostTargetOnce = true;

            // if target is NOT lost, update "_lastKnownTargetPosition"
            if (!_lostTargetOnce)
                _lastKnownTargetPosition = _targetHitloc.GetHitloc().position;

            // if we reach target on next frame and it is not lost, invoke OnReachTarget
            if (VectorHelper.DistanceSquared(this.transform.position, _lastKnownTargetPosition) < Mathf.Pow(speedWithAppliedTime, 2))
            {
                if (_lostTargetOnce)
                {
                    //Debug.Log("_lostTargetOnce. Manual destroying...");
                    Destroy(this.gameObject);
                }
                else
                {
                    //Debug.Log("OnReachTarget");
                    OnReachTarget?.Invoke(AbilityTargetProjectile, this);
                }

                return;
            }

            var direction = (_lastKnownTargetPosition - this.transform.position).normalized;
            var directionWithAppliedSpeed = this.transform.position + direction * speedWithAppliedTime;

            this.transform.position = directionWithAppliedSpeed;
        }
    }
}