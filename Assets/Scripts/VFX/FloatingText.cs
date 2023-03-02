using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        private Camera _camera;
        private RectTransform rectTransform;

        [SerializeField] private string _Text = "";
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                TextChanged?.Invoke(_Text);
            }
        }

        public event Action<string> TextChanged;

        private Vector3 _initialScale;
        public bool AutoScale = true;

        private void Start()
        {
            _camera = Camera.main;
            rectTransform = this.GetComponent<RectTransform>();

            _initialScale = rectTransform.localScale;

            TextChanged += FloatingText_TextChanged;
        }

        private void FloatingText_TextChanged(string text)
        {
            _textMeshProUGUI.text = text;
        }

        public void UpdateText()
        {
            _textMeshProUGUI.text = Text;
        }

        private void LateUpdate()
        {
            if (AutoScale)
            {
                var distanceToCamera = Vector3.Distance(this.transform.position, _camera.transform.position);

                float scale = (float) 0.1 * distanceToCamera;
                rectTransform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                rectTransform.localScale = _initialScale;
            }

            this.transform.rotation = _camera.transform.rotation;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FloatingText_TextChanged(Text);
        }
#endif
    }
}