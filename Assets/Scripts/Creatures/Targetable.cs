using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities.Specific;
using Assets.Scripts.Health;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Assets.Scripts.Creatures
{
    public class Targetable : MonoBehaviour, ITargetable
    {
        public IBaseCreature IBaseCreature { get; private set; }

        [Header("General")]
        [SerializeField] private float _AutoAttackIndicatorHeightValue = 2.1f;

        public float AutoAttackIndicatorHeight => _AutoAttackIndicatorHeightValue;

        // [SerializeField] private Transform _indicatorBone;
        // public Transform IndicatorBone => _indicatorBone;

        [Header("Debug")]
        public bool DisplayIndicatorLocation = false;

        public List<AbilityRangeProjectile> ProjectilesOnTheWay { get; } = new List<AbilityRangeProjectile>();

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (DisplayIndicatorLocation)
                {
                    // var startupIndicatorPos = IndicatorBone == null ? this.transform.position: IndicatorBone.position;
                    var startupIndicatorPos = this.transform.position;
                    var autoAttackIndicatorPos = startupIndicatorPos + new Vector3(0, AutoAttackIndicatorHeight, 0);

                    // SceneView sceneView = SceneView.lastActiveSceneView;
                    // Vector3 autoAttackIndicatorWorldToScreen = sceneView.camera.WorldToScreenPoint(autoAttackIndicatorPos);

                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(autoAttackIndicatorPos, 0.2f);
                }
            }
#endif
        }

        private void Awake()
        {
            IBaseCreature = GetComponent<IBaseCreature>();
        }

        private bool _CanBeAttacked = true;
        public bool CanBeAttacked
        {
            get => _CanBeAttacked;
            set
            {
                bool valueIsDifferentFromBefore = _CanBeAttacked != value;

                _CanBeAttacked = value;

                if (valueIsDifferentFromBefore)
                    CanBeAttackedChanged?.Invoke(value);
            }
        }
        public event Action<bool> CanBeAttackedChanged;

        private bool _CanBeTargeted = true;
        public bool CanBeTargeted
        {
            get => _CanBeTargeted;
            set
            {
                bool valueIsDifferentFromBefore = _CanBeTargeted != value;

                _CanBeTargeted = value;

                if (valueIsDifferentFromBefore)
                    CanBeTargetedChanged?.Invoke(value);
            }
        }
        public event Action<bool> CanBeTargetedChanged;

        public string NameWhenTargeted => IBaseCreature.CreatureSO.Name;
        public bool HasFrontAndBack => IBaseCreature.CreatureSO.HasFrontAndBack;
        public bool CatchAOEByCapsuleCollider => IBaseCreature.CreatureSO.CatchAOEByCapsuleCollider;

        public BaseHealth Health => IBaseCreature.Health;
    }
}