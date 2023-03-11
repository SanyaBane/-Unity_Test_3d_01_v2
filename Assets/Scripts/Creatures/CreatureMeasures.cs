using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [ExecuteInEditMode]
    public class CreatureMeasures : MonoBehaviour
    {
        [SerializeField] private float _radius;
        public float Radius => _radius;

        [SerializeField] private float _height;
        public float Height => _height;

        private CapsuleCollider _capsuleCollider;

        private void Start()
        {
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            _capsuleCollider = this.GetComponent<CapsuleCollider>();

            _capsuleCollider.radius = Radius;
            _capsuleCollider.height = Height;
            // _capsuleCollider.center = new Vector3(0, Height / 2, 0);
            _capsuleCollider.center = new Vector3(0, Height / 2, 0);
        }
        
        // private void OnEnable()
        // {
        //     UpdateInfo();
        // }
    }
}