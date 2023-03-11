using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Creatures;
using Assets.Scripts.NPC;
using Assets.Scripts.NPC.PartyMember.CombatTactics.ShivaLevel;
using Assets.Scripts.NPC.PartyMember.PeaceTactics;
using Assets.Scripts.NPC.ShivaBoss;
using Assets.Scripts.NPC.Tactics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels
{
    public enum enFightPhase
    {
        None,
        StartFight, // Single "Absolute Zero" cast
        FirstBitingOrDrivingCast,
        
    }
    
    public class ShivaLevelManager : BaseLevelManager
    {
        [Header("General")]
        public GameObject MirrorPrefab;

        public MirrorPositions MirrorPositions;
        public DiamondDustPositions DiamondDustPositions;
        public TankingPositions TankingPositions;
        public Vector3 BossPositionAtCenter;

        private Shiva_MirrorMirror3 _mirrorNorthScript;
        private Shiva_MirrorMirror3 _mirrorEastScript;
        private Shiva_MirrorMirror3 _mirrorWestScript;

        public CreatureInfoContainer ShivaBoss;
        public NpcBaseCreature ShivaBossCreature => (NpcBaseCreature)ShivaBoss.BaseCreature;
        public ShivaAI ShivaBossAI { get; private set; }

        [SerializeField] private List<GameObject> objectsToReset = new List<GameObject>();

        public float TimeAfterBossPull { get; private set; } = 0;
        public bool IsBattleStarted { get; private set; } = false;

        public enFightPhase FightPhase = enFightPhase.None;
        
        public event Action BossPooled;
        public event Action BattleEnded;

        public float LevelCircleAreaRadius = 13.8f;

        public Vector3 PositionWhereToTankBoss { get; private set; }
        
        #region Debug
        [Header("Debug")]
        [SerializeField] private bool _displayMirrorAbilityRange;

        private bool? _previousDisplayMirrorAbilityRange = null;
        public bool DisplayMirrorAbilityRange
        {
            get => _displayMirrorAbilityRange;
            set
            {
                if (_displayMirrorAbilityRange == value)
                    return;

                _previousDisplayMirrorAbilityRange = _displayMirrorAbilityRange;
                _displayMirrorAbilityRange = value;
                DisplayMirrorAbilityRangeChange?.Invoke(_displayMirrorAbilityRange);
            }
        }
        
        public event Action<bool> DisplayMirrorAbilityRangeChange;

        [SerializeField] private List<CreatureInfoContainer> TestPlayerPartyMembers = new List<CreatureInfoContainer>();
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_previousDisplayMirrorAbilityRange != _displayMirrorAbilityRange)
            {
                _previousDisplayMirrorAbilityRange = _displayMirrorAbilityRange;
                DisplayMirrorAbilityRangeChange?.Invoke(_displayMirrorAbilityRange);
            }
        }
#endif

        private void Awake()
        {
            ShivaBossAI = ShivaBossCreature.GetComponent<ShivaAI>();

            ShivaBossAI.EnteredFirstPhase += OnEnteredFirstPhase;
            
            ShivaBossCreature.CombatInfoHandler.CombatInfosRemoved += CombatInfoHandlerOnCombatInfosRemoved;
        }

        private void Start()
        {
            InitializeTestParty();
        }

        private void InitializeTestParty()
        {
            if (TestPlayerPartyMembers.Count > 0)
            {
                if (!PartyEntity.CanBeAddedToParty(GameManager.Instance.PlayerCreature))
                    return;
                
                var playerParty = new PartyEntity(GameManager.Instance.PlayerCreature);

                foreach (var creatureInfoContainer in TestPlayerPartyMembers)
                {
                    if (!creatureInfoContainer.gameObject.activeInHierarchy)
                        continue;
                    
                    playerParty.InviteToParty(creatureInfoContainer.BaseCreature);
                }
            }
        }
        
        private void CombatInfoHandlerOnCombatInfosRemoved(CombatInfo combatInfo)
        {
            if (ShivaBossCreature.CombatInfoHandler.CombatInfosCount == 0)
            {
                ResetLevel();
            }
        }

        private void Update()
        {
            TimeAfterBossPull += Time.deltaTime;
            
            if (!IsBattleStarted)
            {
                FightPhase = enFightPhase.None;
                return;
            }

            if (FightPhase == enFightPhase.None)
                FightPhase = enFightPhase.StartFight;
            
            if (TimeAfterBossPull > 4)
            {
                FightPhase = enFightPhase.FirstBitingOrDrivingCast;
            }
        }
        
        public override BaseAICombatTactics GetCombatTactics(NpcAI npcAI)
        {
            switch (npcAI.Role)
            {
                case ERole.MT:
                    return new ShivaFightMainTankCombatTactics(npcAI, this);
                case ERole.OT:
                    return new CombatTacticsAutoAttack(npcAI);
                case ERole.R1:
                case ERole.R2:
                case ERole.M1:
                case ERole.M2:
                    switch (npcAI.INpcBaseCreature.CurrentJob)
                    {
                        case EJob.BLM:
                            return new ShivaFightBLMCombatTactics(npcAI, this);
                        case EJob.DRG:
                            return new ShivaFightDRGCombatTactics(npcAI, this);
                        case EJob.WHM:
                        case EJob.PAL:
                        case EJob.WAR:
                            throw new ArgumentException($"{npcAI.INpcBaseCreature.CurrentJob} can not have role '{npcAI.Role}'");
                        case EJob.None:
                        default:
                            throw new ArgumentException($"{nameof(EJob)} - {npcAI.INpcBaseCreature.CurrentJob}");
                    }
                case ERole.H1:
                case ERole.H2:
                    // todo
                    return new CombatTacticsAutoAttack(npcAI);
                case ERole.None:
                    return new CombatTacticsAutoAttack(npcAI);
                default:
                    throw new ArgumentException($"{nameof(ERole)} - {npcAI.Role}");
            }
        }
        
        public override BaseAIPeaceTactics GetPeaceTactics(NpcAI npcAI)
        {
            switch (npcAI.Role)
            {
                case ERole.MT:
                    return new MainTankPeaceTactics(npcAI);
                case ERole.R1:
                case ERole.R2:
                case ERole.M1:
                case ERole.M2:
                case ERole.OT:
                case ERole.H1:
                case ERole.H2:
                case ERole.None:
                    return new PeaceTacticsDoNothing(npcAI);
                default:
                    throw new ArgumentException($"{nameof(ERole)} - {npcAI.Role}");
            }
        }

        private void OnEnteredFirstPhase()
        {
            IsBattleStarted = true;
            TimeAfterBossPull = 0;

            PositionWhereToTankBoss = BossPositionAtCenter;

            BossPooled?.Invoke();
        }

        // private void Start()
        // {
        //     // MirrorMirror3();
        // }

        // Summons 3 mirrors - 2 yellow and 1 red, which will repeat last Shiva's skill - "Biting Frost" or "Divide Frost".
        public void MirrorMirror3Summon()
        {
            var mirrorNorth = Instantiate(MirrorPrefab, MirrorPositions.MirrorNorthTransform.position, MirrorPositions.MirrorNorthTransform.rotation);
            _mirrorNorthScript = mirrorNorth.GetComponent<Shiva_MirrorMirror3>();

            var mirrorEast = Instantiate(MirrorPrefab, MirrorPositions.MirrorEastTransform.position, MirrorPositions.MirrorEastTransform.rotation);
            _mirrorEastScript = mirrorEast.GetComponent<Shiva_MirrorMirror3>();

            var mirrorWest = Instantiate(MirrorPrefab, MirrorPositions.MirrorWestTransform.position, MirrorPositions.MirrorWestTransform.rotation);
            _mirrorWestScript = mirrorWest.GetComponent<Shiva_MirrorMirror3>();

            objectsToReset.Add(mirrorNorth);
            objectsToReset.Add(mirrorEast);
            objectsToReset.Add(mirrorWest);

            _mirrorNorthScript.SetupMirror(Shiva_MirrorMirror3.MirrorMirror3TypeEnum.Yellow, ShivaBossCreature);


            int mirrorChance = Random.Range(1, 2 + 1);
            // Debug.Log($"mirrorChance: {mirrorChance}");

            if (mirrorChance == 1)
            {
                _mirrorEastScript.SetupMirror(Shiva_MirrorMirror3.MirrorMirror3TypeEnum.Yellow, ShivaBossCreature);
                _mirrorWestScript.SetupMirror(Shiva_MirrorMirror3.MirrorMirror3TypeEnum.Red, ShivaBossCreature);
            }
            else
            {
                _mirrorEastScript.SetupMirror(Shiva_MirrorMirror3.MirrorMirror3TypeEnum.Red, ShivaBossCreature);
                _mirrorWestScript.SetupMirror(Shiva_MirrorMirror3.MirrorMirror3TypeEnum.Yellow, ShivaBossCreature);
            }
        }

        public void MirrorMirror3FirstCastBitingOrDriving(AbilitySO castedBitingOrDrivingAbilitySO)
        {
            if (_mirrorNorthScript != null)
                _mirrorNorthScript.StartCastingBitingOrDriving(castedBitingOrDrivingAbilitySO);

            if (_mirrorEastScript != null)
                _mirrorEastScript.StartCastingBitingOrDriving(castedBitingOrDrivingAbilitySO);

            if (_mirrorWestScript != null)
                _mirrorWestScript.StartCastingBitingOrDriving(castedBitingOrDrivingAbilitySO);
        }

        public void ResetLevel()
        {
            IsBattleStarted = false;
            
            _mirrorNorthScript = null;
            _mirrorEastScript = null;
            _mirrorWestScript = null;

            for (int i = objectsToReset.Count - 1; i >= 0; i--)
            {
                var obj = objectsToReset[i];

                if (obj != null)
                {
                    var creatureInfoContainer = obj.GetComponent<CreatureInfoContainer>();
                    creatureInfoContainer.BaseCreature.DestroyCreature();

                    // Destroy(obj);
                }

                objectsToReset.RemoveAt(i);
            }
            
            BattleEnded?.Invoke();
        }
    }
}