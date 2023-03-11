using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class TargetAutoAttackIndicator : MonoBehaviour
    {
        public Transform ArrowsTransform;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private float CameraDistanceLimit = 0.8f;
        //[SerializeField] private float MaxCameraDistanceHeight = 6f;
        //[SerializeField] private float MinCameraDistanceHeight = 6f;

        public float RotationSpeed = 150f;
        public bool EnableRotation = true;
        public bool Clockwise = true;

        public ITargetable Target { get; private set; }

        private Camera _camera;

        // TODO Данный индикатор должен быть сверху от хп-бара цели, но масштабируется он немного не так как хп-бар.
        // Надо смотреть и тестить в самой финалке.

        public void SetupAndStart(ITargetable target)
        {
            Target = target;

            _camera = Camera.main;

            UpdateInfo();
        }

        private void LateUpdate()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (Target.IBaseCreature.GetRootObjectTransform() == null)
            {
                Debug.LogError($"{nameof(Target.IBaseCreature.GetRootObjectTransform)} == null");
                return;
            }

            // var startupIndicatorPos = Target.IndicatorBone == null ? Target.IBaseCreature.GetRootObjectTransform().position : Target.IndicatorBone.position;
            var startupIndicatorPos = Target.IBaseCreature.GetRootObjectTransform().position;
            var autoAttackIndicatorPos = startupIndicatorPos + new Vector3(0, Target.AutoAttackIndicatorHeight, 0);

            // var distanceToCamera = Vector3.Distance(autoAttackIndicatorPos, _camera.transform.position);
            var distanceToCameraSquared = VectorHelper.DistanceSquared(autoAttackIndicatorPos, _camera.transform.position);
            if (distanceToCameraSquared < Mathf.Pow(CameraDistanceLimit, 2))
            {
                _canvasGroup.alpha = 0;
            }
            else
            {
                _canvasGroup.alpha = 1;
            }

            this.transform.position = _camera.WorldToScreenPoint(autoAttackIndicatorPos);

            if (EnableRotation)
            {
                Vector3 rotationAxisDirection = Clockwise ? Vector3.back : Vector3.forward;

                ArrowsTransform.transform.localRotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, rotationAxisDirection);
            }
        }
    }
}