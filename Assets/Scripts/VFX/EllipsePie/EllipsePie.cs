using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HelpersUnity;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class EllipsePie : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject _gameObjectWithMaterial;

        public Vector3 GOWithMaterialInitialRotation = new Vector3(90, 0, 0);

        public Texture2D Texture;

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

        public float SpawnTime = 0.3f;

        public bool DestroyOnTimePassed = true;
        public float Lifetime = 1.0f;

        public float FadeInTime = 0.3f;

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
        
        [Header("Debug (readonly)")]
        [SerializeField] private Color _fadeInColor;

        [SerializeField] private float _passedTime;
        [SerializeField] private bool _isStartedFadeIn;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_gameObjectWithMaterial == null)
            {
                Debug.LogError($"{nameof(_gameObjectWithMaterial)} == null.");
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

            _gameObjectWithMaterial.transform.localRotation = Quaternion.Euler(GOWithMaterialInitialRotation.x, GOWithMaterialInitialRotation.y, GOWithMaterialInitialRotation.z - ClockwiseRotation);
        }

        public void UpdateShaderValues()
        {
            _renderer.GetPropertyBlock(_propBlock);
            // _renderer.sharedMaterial.SetFloat("_AmountDegrees", Angle);
            _propBlock.SetFloat("_AmountDegrees", Angle);
            UpdateColor(false);

            if (Texture != null)
                _propBlock.SetTexture("_MainTex", Texture);

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

            if (DestroyOnTimePassed && _passedTime >= Lifetime + SpawnTime && !_isStartedFadeIn)
            {
                _isStartedFadeIn = true;
                FadeIn(FadeInTime);
            }
        }

        private void FadeIn(float time)
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

            Destroy(this.gameObject);
        }

        private void LateUpdate()
        {
            if (Parent != null)
            {
                this.transform.position = Parent.transform.position;
                this.transform.rotation = Parent.transform.rotation;
            }
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