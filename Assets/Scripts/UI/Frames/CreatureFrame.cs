using Assets.Scripts.Interfaces;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class CreatureFrame : BaseUIFrame
    {
        [SerializeField] private TextMeshProUGUI _ownerName_TMP;
        [SerializeField] private HealthBarGUI _healthBarGUI;
        [SerializeField] private ManaBarGUI _manaBarGUI;

        private CreatureBuffsUI _creatureBuffsUI;
        public IBaseCreature CurrentFrameOwner { get; protected set; }

        private bool _isInitialized = false;

        private string _OwnerName;
        public string OwnerName
        {
            get => _OwnerName;
            protected set
            {
                _OwnerName = value;

                _ownerName_TMP.text = value;
            }
        }

        protected virtual void Start()
        {
            Init();
        }
        
        public override void Setup()
        {
            Init();
        }

        public void Init()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            _creatureBuffsUI = GetComponentInChildren<CreatureBuffsUI>();
            if (_creatureBuffsUI == null)
                Debug.LogError($"{nameof(_creatureBuffsUI)} == null");

            _creatureBuffsUI.SetupCreatureFrame(this);
        }

        protected virtual void Update()
        {
        }

        public virtual void SetNewOwner(IBaseCreature newFrameOwner)
        {
            // if target changed for same target as before (is it even possible?), then do nothing
            if (newFrameOwner != null && CurrentFrameOwner == newFrameOwner)
                return;

            if (CurrentFrameOwner != null)
                OldOwnerUnsubscribe(CurrentFrameOwner);

            if (newFrameOwner == null)
            {
                CurrentFrameOwner = null;
            }
            else
            {
                CurrentFrameOwner = newFrameOwner;

                OwnerName = CurrentFrameOwner.ITargetable.NameWhenTargeted;

                NewOwnerSubscribe(CurrentFrameOwner);
            }

            _healthBarGUI.SetNewOwner(CurrentFrameOwner);

            if (_manaBarGUI != null)
                _manaBarGUI.SetNewOwner(CurrentFrameOwner);
        }

        protected virtual void NewOwnerSubscribe(IBaseCreature newFrameOwner)
        {
            if (_creatureBuffsUI == null)
            {
                Debug.LogError("ebal");
            }

            _creatureBuffsUI.NewOwnerSubscribe(newFrameOwner);
        }

        protected virtual void OldOwnerUnsubscribe(IBaseCreature oldFrameOwner)
        {
            _creatureBuffsUI.OldOwnerUnsubscribe(oldFrameOwner);
        }
    }
}