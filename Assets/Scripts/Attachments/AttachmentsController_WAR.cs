using UnityEngine;

namespace Assets.Scripts.Attachments
{
    public class AttachmentsController_WAR : AttachmentsController
    {
        
        [SerializeField] private Transform _attach_Weapon2HSheathed;
        [SerializeField] private Transform _attach_Weapon2HReady;
        
        [SerializeField] private Transform _attach_Weapon1HRightSheathed;
        [SerializeField] private Transform _attach_Weapon1HRightReady;
        
        [SerializeField] private Transform _attach_Weapon1HLeftSheathed;
        [SerializeField] private Transform _attach_Weapon1HLeftReady;
        
        [SerializeField] private Transform _attach_ShieldSheathed;
        [SerializeField] private Transform _attach_ShieldReady;

        public Transform Attach_Weapon2HSheathed => this._attach_Weapon2HSheathed != null ? this._attach_Weapon2HSheathed : this.transform;
        public Transform Attach_Weapon2HReady => this._attach_Weapon2HReady != null ? this._attach_Weapon2HReady : this.transform;
        
        public Transform Attach_Weapon1HRightSheathed => this._attach_Weapon1HRightSheathed != null ? this._attach_Weapon1HRightSheathed : this.transform;
        public Transform Attach_Weapon1HRightReady => this._attach_Weapon1HRightReady != null ? this._attach_Weapon1HRightReady : this.transform;
        
        public Transform Attach_Weapon1HLeftSheathed => this._attach_Weapon1HLeftSheathed != null ? this._attach_Weapon1HLeftSheathed : this.transform;
        public Transform Attach_Weapon1HLeftReady => this._attach_Weapon1HLeftReady != null ? this._attach_Weapon1HLeftReady : this.transform;
        
        public Transform Attach_ShieldSheathed => this._attach_ShieldSheathed != null ? this._attach_ShieldSheathed : this.transform;
        public Transform Attach_ShieldReady => this._attach_ShieldReady != null ? this._attach_ShieldReady : this.transform;
    }
}