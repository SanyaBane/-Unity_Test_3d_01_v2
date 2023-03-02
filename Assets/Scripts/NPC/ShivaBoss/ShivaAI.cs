using System;
using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Buffs;
using Assets.Scripts.Buffs.ScriptableObjects;
using Assets.Scripts.Levels;
using Assets.Scripts.NPC.Tactics;
using UnityEngine;

namespace Assets.Scripts.NPC.ShivaBoss
{
    public class ShivaAI : NpcAI
    {
        [SerializeField] private float _enrageTimerSeconds = 841;
        public float EnrageTimerSeconds => _enrageTimerSeconds;

        public const string ABILITY_ID_SHIVA_ABSOLUTE_ZERO = "Shiva_AbsoluteZero";
        public const string ABILITY_ID_SHIVA_MIRROR_MIRROR = "Shiva_MirrorMirror";
        public const string ABILITY_ID_SHIVA_BITING_FROST = "Shiva_BitingFrost";
        public const string ABILITY_ID_SHIVA_DRIVING_FROST = "Shiva_DrivingFrost";

        [Header("ShivaAI")]
        [SerializeField] private ShivaLevelManager shivaLevelManager;

        public ShivaLevelManager ShivaLevelManager => shivaLevelManager;

        public BaseBuffSO TestPermanentBuff;

        public event Action EnteredFirstPhase;

        public void RaiseEnteredFirstPhase()
        {
            EnteredFirstPhase?.Invoke();
        }

        protected override void Start()
        {
            base.Start();

            var testPermanentBuffAbilityParameters = new AbilityParameters()
            {
                DefaultAbilityParameters = new DefaultAbilityParameters(INpcBaseCreature, INpcBaseCreature.ITargetable)
            };

            _buffsController.CreateAndAddBuff(TestPermanentBuff, null, testPermanentBuffAbilityParameters, false);
        }
        
        protected override void SetPeaceTactics()
        {
            PeaceTacticsAI = new PeaceTacticsDoNothing(this);
        }

        protected override void SetCombatTactics()
        {
            CombatTacticsAI = new CombatTacticsAutoAttack(this);
        }

        protected override void SetupCombatState()
        {
            baseCombatState = new ShivaFirstPhaseCombatState(this, INpcBaseCreature, _animator, _abilitiesController, _autoAttack);

            _stateMachine.AddAnyTransition(baseCombatState, FoundTargetToAttack());
            _stateMachine.AddTransition(baseCombatState, returnToPositionBeforeCombatState, LostTargetToAttack());
            _stateMachine.AddTransition(returnToPositionBeforeCombatState, idleState, returnToPositionBeforeCombatState.CanSwitchToIdle());
        }

        // protected override void OnEnteredReturnToPositionBeforeCombatState()
        // {
        //     base.OnEnteredReturnToPositionBeforeCombatState();
        //
        //     ShivaLevelManager.ResetLevel();
        // }
    }
}