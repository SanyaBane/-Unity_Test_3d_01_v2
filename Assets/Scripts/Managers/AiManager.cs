using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Assets.Scripts.Creatures;
using Assets.Scripts.Factions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Levels;
using UnityEngine;

namespace Assets.Scripts
{
    public class AiManager : MonoBehaviour
    {
        [SerializeField] private BaseLevelManager _specialLevelManager;

        private BaseLevelManager _defaultLevelManager;
        public BaseLevelManager BaseLevelManager
        {
            get
            {
                if (_specialLevelManager != null)
                    return _specialLevelManager;

                if (_defaultLevelManager == null)
                {
                    var baseLevelManagerGO = new GameObject("BaseLevelManager");
                    baseLevelManagerGO.transform.SetParent(this.transform);
                    var baseLevelManager = baseLevelManagerGO.AddComponent<BaseLevelManager>();
                    
                    _defaultLevelManager = baseLevelManager;
                }

                return _defaultLevelManager;
            }
        }
    }
}