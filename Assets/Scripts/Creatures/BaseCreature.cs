using Assets.Scripts.Factions;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Attachments;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Creatures.Combat;
using Assets.Scripts.Enums;
using Assets.Scripts.Health;
using Assets.Scripts.Managers;
using Assets.Scripts.ManaSystem;
using Assets.Scripts.Stats;
using Assets.Scripts.TargetHandling;
using QuickOutline;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Creatures
{
    [DebuggerDisplay("Owner: {CreatureInfoContainer.gameObject.name}")]
    [RequireComponent(typeof(BaseHealth), typeof(Targetable))]
    public abstract class BaseCreature : MonoBehaviour, IBaseCreature
    {
        [SerializeField] private CreatureSO _creatureSO;
        public CreatureSO CreatureSO => _creatureSO;

        public CombatInfoHandler CombatInfoHandler { get; private set; }

        public enum FactionStartupAssignmentEnum
        {
            AutoFromSO,
            Manual
        }

        [Header("Faction")]
        [SerializeField] private FactionStartupAssignmentEnum FactionStartupAssignment = FactionStartupAssignmentEnum.AutoFromSO;

        [SerializeField] private Faction _Faction;

        public Faction Faction
        {
            get => _Faction;
            protected set { _Faction = value; }
        }

        [Header("General")]
        [SerializeField] private CreatureAttitudeEnum _creatureAttitude = CreatureAttitudeEnum.Mob;

        public CreatureAttitudeEnum CreatureAttitude => _creatureAttitude;

        [SerializeField] private EJob _currentJob;
        public EJob CurrentJob => _currentJob;

        public CreatureInfoContainer CreatureInfoContainer { get; private set; }

        public BaseHealth Health { get; private set; }
        public BaseManaController ManaController { get; private set; }
        public PartyController PartyController { get; private set; }

        public CreatureMeasures CreatureMeasures { get; private set; }

        public Animator Animator { get; set; }
        public AttachmentsController AttachmentsController { get; set; }
        public AbilitiesController AbilitiesController { get; set; }
        public BuffsController BuffsController { get; set; }
        public StatsController StatsController { get; set; }
        public AutoAttackController AutoAttackController { get; set; }
        public ICanSelectTarget ICanSelectTarget { get; set; }
        public TargetHandler TargetHandler { get; set; }

        public IBaseCreature IBaseCreature => this;

        public event Action BeforeCreatureDestroy;

        //public List<CombatInfo> CombatInteractionWithCreatures { get; private set; } = new List<CombatInfo>();

        public ITargetable ITargetable { get; private set; }

        public Outline Outline { get; private set; }

        #region IBaseCreature

        public bool IsGrounded { get; set; } = true;
        public bool IsMoving { get; set; } = false;
        public bool IsTurning { get; set; } = false;

        public List<IBaseCreature> ListAttackedByCreatures = new List<IBaseCreature>();

        #endregion

        #region IHaveHitloc

        public Transform GetHitloc() => AttachmentsController != null ? AttachmentsController.Attach_Hitloc : GetRootObjectTransform();

        #endregion

        #region IHasTransform

        /// <summary>
        /// Will return null if GameObject is "null".
        /// </summary>
        /// <returns></returns>
        public Transform GetRootObjectTransform() => CreatureInfoContainer == null ? null : CreatureInfoContainer.transform;

        public Vector3 GetGroundedPosition() => IsGrounded ? GetRootObjectTransform().position : CalculateGroundedPosition();

        private Vector3 CalculateGroundedPosition()
        {
            var ray = new Ray(GetRootObjectTransform().position, -Vector3.up);
            if (Physics.Raycast(ray, out var hit, 5.0f, GameManager.Instance.GroundedLayerMask))
            {
                return hit.point;
            }

            return GetRootObjectTransform().position;
        }

        #endregion


        protected virtual void Awake()
        {
            CreatureInfoContainer = GetComponentInParent<CreatureInfoContainer>();

            CreatureMeasures = GetComponentInChildren<CreatureMeasures>();
            if (CreatureMeasures == null)
                Debug.LogError($"{nameof(CreatureMeasures)} not found.");

            Health = GetComponent<BaseHealth>();
            ManaController = GetComponent<BaseManaController>();
            PartyController = GetComponent<PartyController>();
            ITargetable = GetComponent<Targetable>();

            Outline = GetComponent<Outline>();

            UpdateCreatureMeshComponents();

            AbilitiesController = GetComponent<AbilitiesController>();
            BuffsController = GetComponent<BuffsController>();
            StatsController = GetComponent<StatsController>();
            AutoAttackController = GetComponent<AutoAttackController>();
            ICanSelectTarget = GetComponent<ICanSelectTarget>();
            TargetHandler = GetComponent<TargetHandler>();

            CombatInfoHandler = new CombatInfoHandler(this);
        }
        
        protected virtual void Start()
        {
            if (FactionStartupAssignment == FactionStartupAssignmentEnum.AutoFromSO)
                Faction = _creatureSO.DefaultFaction;

            UpdateJob();
        }

        private void UpdateCreatureMeshComponents()
        {
            UpdateAnimator();
            UpdateAttachmentsController();
        }

        private void UpdateAnimator()
        {
            var animators = CreatureInfoContainer.GetComponentsInChildren<Animator>(includeInactive: false);
            if (animators.Length == 0)
            {
                Debug.LogError($"Component '{nameof(Animator)}' not found on gameObject with name: {gameObject.name}.");
            }
            else if (animators.Length > 1)
            {
                Debug.LogError($"Founded more then 1 components '{nameof(Animator)}' on gameObject with name: {gameObject.name}.");
            }
            else
            {
                Animator = animators[0];
            }
        }

        private void UpdateAttachmentsController()
        {
            var attachmentControllers = CreatureInfoContainer.GetComponentsInChildren<AttachmentsController>(includeInactive: false);
            if (attachmentControllers.Length == 0)
            {
                Debug.LogError($"Component '{nameof(AttachmentsController)}' not found on gameObject with name: {gameObject.name}.");
            }
            else if (attachmentControllers.Length > 1)
            {
                Debug.LogError($"Founded more then 1 components '{nameof(AttachmentsController)}' on gameObject with name: {gameObject.name}.");
            }
            else
            {
                AttachmentsController = attachmentControllers[0];
            }
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void UpdateJob()
        {
            Animator.SetInteger(ConstantsAnimator.PLAYER_CREATURE_INTEGER_CURRENT_JOB, (int) CurrentJob);
        }

        public void DestroyCreature(float delay = 0f)
        {
            BeforeCreatureDestroy?.Invoke();

            ITargetable.CanBeTargeted = false;

            Destroy(CreatureInfoContainer.gameObject, delay);
        }
    }
}