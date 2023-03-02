using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HelpersUnity;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class EllipseProjection : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject _gameObjectWithMaterial;

        public Vector3 GOWithMaterialInitialRotation = new Vector3(90, 0, 0);

        
        
        public Transform Parent;

        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;

        [Range(0, 360)]
        public float Angle = 90;

        [Range(0, 360)]
        public float ClockwiseRotation = 0;

        private const float MIN_RADIUS = 0;
        public float Radius = 3;

        private const float MIN_HEIGHT = 0;
        public float Height = 2;

        public float SpawnTime = 0.5f;
        
        public float StartRippleIntervalOffset = 0.1f;
        public float RippleIntervalOffset = 0.5f;

        [Range(0, 0.1f)]
        public float RippleSpeed = 0.05f;

        [SerializeField] private Color _color = Color.white;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateColor(true);
            }
        }
        
        public bool AnimateRippleDistance = true;

        [Header("Debug (readonly)")]
        [SerializeField] private Color _fadeInColor;

        [SerializeField] private float _ripplePassedInterval = 0;
        [SerializeField] private float _passedTime = 0;
        [SerializeField] private float _rippleDistance = 0;

        private bool IsInstantiated => this.gameObject.scene.name != null;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_gameObjectWithMaterial == null)
            {
                if (IsInstantiated)
                {
                    Debug.LogError($"{nameof(_gameObjectWithMaterial)} == null.");
                }

                return;
            }

            if (_renderer == null)
                _renderer = _gameObjectWithMaterial.GetComponent<Renderer>();

            if (_propBlock == null)
                _propBlock = new MaterialPropertyBlock();
        }

        public void UpdateValues()
        {
            Init();

            if (!IsInstantiated)
                return;

            // _passedTime = 0; // restart shader

            UpdateShaderValues();

            Radius = Mathf.Max(Radius, MIN_RADIUS);
            Height = Mathf.Max(Height, MIN_HEIGHT);

            if (Application.isPlaying)
            {
                _renderer.transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                _renderer.transform.localScale = new Vector3(Radius * 2, Radius * 2, Height);
            }

            if (_gameObjectWithMaterial != null)
            {
                _gameObjectWithMaterial.transform.localRotation = Quaternion.Euler(GOWithMaterialInitialRotation.x, GOWithMaterialInitialRotation.y, GOWithMaterialInitialRotation.z - ClockwiseRotation);
            }

            _ripplePassedInterval = -1 * (StartRippleIntervalOffset * Radius);
        }

        public void UpdateShaderValues()
        {
            _renderer.GetPropertyBlock(_propBlock);
            
            _propBlock.SetFloat("_AmountDegrees", Angle);
            UpdateColor(false);

            if (AnimateRippleDistance)
                _propBlock.SetFloat("_RippleDistance", _rippleDistance);

            _renderer.SetPropertyBlock(_propBlock);
        }
        
        private void UpdateColor(bool isSetPropertyBlock)
        {
            _propBlock.SetColor("_Color", Color);

            if (isSetPropertyBlock)
            {
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        private void Update()
        {
            _passedTime += Time.deltaTime;

            if (_passedTime < SpawnTime)
            {
                var t = _passedTime / SpawnTime;
                _renderer.transform.localScale = new Vector3(Radius * 2 * t, Radius * 2 * t, Height);
            }
            else
            {
                _renderer.transform.localScale = new Vector3(Radius * 2, Radius * 2, Height);
            }

            if (AnimateRippleDistance)
            {
                UpdateRippleDistance();
            }
        }

        private void UpdateRippleDistance()
        {
            _ripplePassedInterval += Time.deltaTime * Radius;

            if (_passedTime >= SpawnTime)
            {
                if (_ripplePassedInterval > 0)
                {
                    if (_ripplePassedInterval >= Radius + RippleIntervalOffset * Radius)
                    {
                        _ripplePassedInterval = 0;
                        _rippleDistance = 0;
                    }
                    else
                    {
                        _rippleDistance += RippleSpeed;
                        // _rippleDistance += RippleSpeed / Radius;
                    }
                }
            }

            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_RippleDistance", _rippleDistance);
            _renderer.SetPropertyBlock(_propBlock);
        }

        public void FadeIn(float time)
        {
            StartCoroutine(FadeInCoroutine(time));
        }

        private IEnumerator FadeInCoroutine(float time)
        {
            float currentPassedTime = 0;
            _fadeInColor = Color;

            while (currentPassedTime < time)
            {
                currentPassedTime += Time.deltaTime;

                float currentPassedTimePercentage = currentPassedTime / time;

                _fadeInColor.a = Color.a - (currentPassedTimePercentage * Color.a);
                // Debug.Log($"currentPassedTimePercentage: {currentPassedTimePercentage}; fadeInColor.a: {fadeInColor.a}");

                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_Color", _fadeInColor);
                _renderer.SetPropertyBlock(_propBlock);

                yield return null;
            }
        }

        private void LateUpdate()
        {
            if (Parent != null)
            {
                var parentTransform = Parent.transform;
                var transform1 = this.transform;
                
                transform1.position = parentTransform.position;
                transform1.rotation = parentTransform.rotation;
            }

            // _projector.transform.localRotation = Quaternion.Euler(_projector.transform.localRotation.eulerAngles.x, 0, -ClockwiseRotation);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Debug.Log($"OnValidate()");
            UpdateValues();
        }
#endif
    }
}