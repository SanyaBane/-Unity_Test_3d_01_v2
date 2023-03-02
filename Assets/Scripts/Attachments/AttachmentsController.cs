using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Attachments
{
    public class AttachmentsController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Transform _attach_Hitloc;

        [SerializeField] private Transform _attach_Attack1;

        public Transform Attach_Hitloc => this._attach_Hitloc != null ? this._attach_Hitloc : this.transform;
        public Transform Attach_Attack1 => this._attach_Attack1 != null ? this._attach_Attack1 : this.transform;

        [Header("Editor")]
        [HideInInspector] public int GOSelectorHelpersCount = 0;
        
        [Serializable]
        public class AttachmentsAndGameObjectsNames
        {
            public string AttachmentName = "";
            public string GameObjectName = "";
        }

        [HideInInspector] public List<AttachmentsAndGameObjectsNames> GetGOSelectorHelpersNames = new List<AttachmentsAndGameObjectsNames>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GOSelectorHelpersCount < 0)
                GOSelectorHelpersCount = 0;
        }
#endif
    }
}