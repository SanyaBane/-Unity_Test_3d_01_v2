using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpinningMovement : MonoBehaviour
{
    private Vector3 spawnPosition;
    private float _cosResult = 0;
    private float _sinResult = 0;
    private float _spawnTime;

    private float _initialDistanceToTarget;
    private float _distanceToTargetWhenReachedStartAmplitude;
    private float _currentAmplitude;
    private bool _reachedStartAmplitude = false;

    [SerializeField] private Transform target;
    [SerializeField] private Transform rotatingObject;

    public float MoveSpeed = 6.0f;
    public float StartAmplitude = 0.7f;
    public float Frequency = 2.0f;
    public float SpeedToReachStartAmplitude = 0.01f;
    public float ReduceAmplitudeOnCloseToTargetDistanceSpeed = 0.08f;

    [Range(0, 1f)]
    public float CloseToTargetDistancePercentage = 0.5f;

    void Start()
    {
        spawnPosition = this.transform.position;

        ResetRotatingObj();
    }

    private void Update()
    {
        float moveSpeedWithTimeApplied = MoveSpeed * Time.deltaTime;
        float currentDistanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
        if (currentDistanceToTarget < moveSpeedWithTimeApplied)
        {
            this.transform.position = spawnPosition;
            ResetRotatingObj();
            //return;
        }

        var directionToTarget = (target.transform.position - this.transform.position).normalized;
        this.transform.position = this.transform.position + (directionToTarget * moveSpeedWithTimeApplied);

        float timeFromSpawn = Time.time - _spawnTime;

        if (_reachedStartAmplitude)
        {
            var spawnDistanceToTargetPercentage = _initialDistanceToTarget * CloseToTargetDistancePercentage;
            if (currentDistanceToTarget <= spawnDistanceToTargetPercentage)
            {
                _currentAmplitude = Mathf.Lerp(
                    _currentAmplitude, 
                    currentDistanceToTarget / _distanceToTargetWhenReachedStartAmplitude * StartAmplitude, 
                    ReduceAmplitudeOnCloseToTargetDistanceSpeed);
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

        rotatingObject.transform.position = new Vector3(this.transform.position.x + _cosResult, this.transform.position.y + _sinResult, this.transform.position.z);
    }

    private void ResetRotatingObj()
    {
        _reachedStartAmplitude = false; // false
        _currentAmplitude = 0;

        var randonVal = Random.Range(0f, Mathf.PI); // чтобы при спавне, снаряд летел в рандомную сторону от точки спавна (влево/вправо/вверх/вниз)
        _spawnTime = Time.time + randonVal;

        _initialDistanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
    }
}
