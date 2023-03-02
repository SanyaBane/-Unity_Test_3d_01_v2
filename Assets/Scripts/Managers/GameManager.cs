using System;
using Assets.Scripts.Creatures;
using Assets.Scripts.HelpersUnity;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public const float MANA_TICK_INTERVAL = 3f;

        public static GameManager Instance { get; private set; }

        public int GroundedLayerMask { get; private set; }
        public int SelectableLayerMask { get; private set; }
        public int NpcAndPlayerLayerMask { get; private set; }

        private GameObject _playerGameObject;
        public GameObject PlayerGameObject
        {
            get
            {
                if (_playerGameObject == null)
                {
                    GameObject playerTagGameObject = GameObject.FindGameObjectWithTag(TagManager.PlayerTag);
                    _playerGameObject = CreatureHelper.GetCreatureInfoContainerFromPlayerTagObject(playerTagGameObject.transform).gameObject;
                }

                return _playerGameObject;
            }
        }

        private PlayerCreature _playerCreature;
        public PlayerCreature PlayerCreature
        {
            get
            {
                if (_playerCreature == null)
                {
                    var playerCreatureInfoContainer = PlayerGameObject.GetComponent<CreatureInfoContainer>();
                    _playerCreature = playerCreatureInfoContainer.BaseCreature.IBaseCreature as PlayerCreature;
                }

                return _playerCreature;
            }
        }

        public Canvas PlayerCanvas;

        public ObjectPool FloatingTextDamagePool;

        private float _manaTickTimer = 0;
        public event Action ManaTicked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            GroundedLayerMask = LayerMask.GetMask(LayerManager.LAYER_NAME_GROUND);
            SelectableLayerMask = LayerMask.GetMask(LayerManager.LAYER_NAME_SELECTABLE);

            NpcAndPlayerLayerMask = LayerMask.GetMask(LayerManager.LAYER_NAME_NPC, LayerManager.LAYER_NAME_PLAYER);
        }

        private void Update()
        {
            _manaTickTimer += Time.deltaTime;
            if (_manaTickTimer >= MANA_TICK_INTERVAL)
            {
                _manaTickTimer = 0;
                ManaTicked?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
#if UNITY_EDITOR
                Debug.Log("Pressed button to stop playing game inside UnityEditor.");
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("This command is not supported outside of UnityEditor");
#endif
            }
        }

        private InputController_WoW _InputController_WoW;
        public InputController_WoW InputController_WoW
        {
            get
            {
                if (_InputController_WoW == null)
                    _InputController_WoW = this.GetComponentInChildren<InputController_WoW>();

                return _InputController_WoW;
            }
        }

        private SceneManager _SceneManager;
        public SceneManager SceneManager
        {
            get
            {
                if (_SceneManager == null)
                    _SceneManager = this.GetComponentInChildren<SceneManager>();

                return _SceneManager;
            }
        }

        // GUI Manager located on the "Canvas" Gameobject
        private GUIManager _GUIManager;
        public GUIManager GUIManager
        {
            get
            {
                if (_GUIManager == null)
                {
                    _GUIManager = PlayerCanvas.GetComponent<GUIManager>();
                }

                return _GUIManager;
            }
        }

        private CursorManager _CursorManager;
        public CursorManager CursorManager
        {
            get
            {
                if (_CursorManager == null)
                    _CursorManager = this.GetComponentInChildren<CursorManager>();

                return _CursorManager;
            }
        }

        private CheatManager _CheatManager;
        public CheatManager CheatManager
        {
            get
            {
                if (_CheatManager == null)
                    _CheatManager = this.GetComponentInChildren<CheatManager>();

                return _CheatManager;
            }
        }

        private SettingsManager _SettingsManager;
        public SettingsManager SettingsManager
        {
            get
            {
                if (_SettingsManager == null)
                    _SettingsManager = this.GetComponentInChildren<SettingsManager>();

                return _SettingsManager;
            }
        }

        private PartyManager _PartyManager;
        public PartyManager PartyManager
        {
            get
            {
                if (_PartyManager == null)
                    _PartyManager = this.GetComponentInChildren<PartyManager>();

                return _PartyManager;
            }
        }

        private AOEManager _AOEManager;
        public AOEManager AOEManager
        {
            get
            {
                if (_AOEManager == null)
                    _AOEManager = this.GetComponentInChildren<AOEManager>();

                return _AOEManager;
            }
        }

        private AiManager _aiManager;
        public AiManager AiManager
        {
            get
            {
                if (_aiManager == null)
                    _aiManager = this.GetComponentInChildren<AiManager>();

                return _aiManager;
            }
        }
    }
}