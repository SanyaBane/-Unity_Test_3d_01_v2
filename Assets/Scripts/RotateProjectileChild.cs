using UnityEngine;

namespace Assets.Scripts
{
    public class RotateProjectileChild : MonoBehaviour
    {
        private float _cosResult = 0;
        private float _sinResult = 0;
        private float _spawnTime;

        private float _initialDistanceToTarget;
        private float _distanceToTargetWhenReachedStartAmplitude;

        [SerializeField] private float _currentAmplitude;
        [SerializeField] private bool _reachedStartAmplitude = false;

        [SerializeField] private Transform _target;
        [SerializeField] private Transform _rotatingObject; // assign from Inspector

        [SerializeField] private bool _turnedOn = false;

        public float MoveSpeed = 6.0f;
        public float StartAmplitude = 0.7f;
        public float Frequency = 2.0f;
        public float SpeedToReachStartAmplitude = 0.01f;
        public float ReduceAmplitudeOnCloseToTargetDistanceSpeed = 0.08f;

        [Range(0, 1f)]
        public float CloseToTargetDistancePercentage = 0.5f;

        private void Update()
        {
            if (!_turnedOn || _target.transform == null)
                return;

            float currentDistanceToTarget = Vector3.Distance(this.transform.position, _target.transform.position);

            float timeFromSpawn = Time.time - _spawnTime;

            var spawnDistanceToTargetPercentage = _initialDistanceToTarget * CloseToTargetDistancePercentage;
            if (currentDistanceToTarget <= spawnDistanceToTargetPercentage)
            {
                if (_reachedStartAmplitude)
                {
                    var lerpGoal = currentDistanceToTarget / _distanceToTargetWhenReachedStartAmplitude * StartAmplitude;
                    _currentAmplitude = Mathf.Lerp(_currentAmplitude, lerpGoal, ReduceAmplitudeOnCloseToTargetDistanceSpeed);
                }
                else
                {
                    var lerpGoal = currentDistanceToTarget / 10 * StartAmplitude;
                    _currentAmplitude = Mathf.Lerp(_currentAmplitude, lerpGoal, ReduceAmplitudeOnCloseToTargetDistanceSpeed);
                }
            }
            else
            {
                _currentAmplitude += SpeedToReachStartAmplitude;

                if (_currentAmplitude >= StartAmplitude)
                {
                    _currentAmplitude = StartAmplitude;
                    _reachedStartAmplitude = true;
                    _distanceToTargetWhenReachedStartAmplitude = currentDistanceToTarget;
                }
            }

            _cosResult = Mathf.Cos(timeFromSpawn * Frequency) * _currentAmplitude;
            _sinResult = Mathf.Sin(timeFromSpawn * Frequency) * _currentAmplitude;

            _rotatingObject.transform.position = new Vector3(this.transform.position.x + _cosResult, this.transform.position.y + _sinResult, this.transform.position.z);
        }

        public void Setup(Transform target)
        {
            _turnedOn = true;

            _target = target;

            _reachedStartAmplitude = false; // false
            _currentAmplitude = 0;

            var randonVal = Random.Range(0f, Mathf.PI); // чтобы при спавне, снаряд летел в рандомную сторону от точки спавна (влево/вправо/вверх/вниз)
            _spawnTime = Time.time + randonVal;

            _initialDistanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
        }
    }
}
