using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class DebugInfoAboveGameObject : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private FloatingText _floatingText;
        public FloatingText FloatingText => _floatingText;

        public bool IsDisplay = false;

        [SerializeField] private bool _autoScale = false;

        private void Awake()
        {
        }

        private void Update()
        {
            if (_floatingText == null)
            {
                Debug.LogError($"{nameof(_floatingText)} == null");
                return;
            }

            if (IsDisplay)
            {
                _canvas.gameObject.SetActive(true);
                _floatingText.UpdateText();
            }
            else
            {
                _canvas.gameObject.SetActive(false);
            }

            //_floatingText.Text = $"Acceleration: {_aiPath.}";

            //_floatingText.SetText($"Velocity: {_navMeshAgent.velocity}");
            //_floatingText.SetText($"Velocity (mag): {_navMeshAgent.velocity.magnitude}");
        }

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_floatingText == null)
            {
                Debug.LogError($"{nameof(_floatingText)} == null");
                return;
            }
            
            _floatingText.AutoScale = _autoScale;
        }
    #endif
    }
}
