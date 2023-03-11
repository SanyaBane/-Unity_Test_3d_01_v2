using Assets.Scripts.Levels;
using UnityEngine;

namespace Assets.Scripts.Managers
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