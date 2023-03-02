using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    // https://youtu.be/mFOi6W7lohk
    public class MoveHorizontalCircle : MonoBehaviour
    {
        [SerializeField] private float _cosResult = 0;
        [SerializeField] private float _sinResult = 0;
        [SerializeField] private Vector3 _positionBeforeRotationStart;

        public float Amplitude = 1.0f;
        public float Frequency = 1.0f;

        void Start()
        {
            _positionBeforeRotationStart = transform.position;
        }

        void Update()
        {
            _cosResult = Mathf.Cos(Time.time * Frequency) * Amplitude;
            _sinResult = Mathf.Sin(Time.time * Frequency) * Amplitude;

            //Debug.Log($"Time.time: {Time.time}; Cos result: {_cosResult}; Sin result: {_sinResult}");

            transform.position = new Vector3(_positionBeforeRotationStart.x + _cosResult, transform.position.y, _positionBeforeRotationStart.z + _sinResult);
        }
    }
}
