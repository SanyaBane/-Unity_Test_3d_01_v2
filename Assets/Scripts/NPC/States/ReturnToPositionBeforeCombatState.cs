using System;
using Assets.Scripts.Buffs;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.StateMachineScripts;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class ReturnToPositionBeforeCombatState : BaseNpcAiState
    {
        private readonly NpcAI _npcAI;
        private readonly Animator _animator;
        private readonly NPCHealth _npcHealth;
        private readonly BuffsController _buffsController;
        private readonly AIPath _aiPath;

        private bool cachedHealthIsInvulnerable;
        private BaseHealth.ImmortalityTypeEnum cachedHealthInvulnerabilityType;
        private bool cachedCanEngage;
        private bool cachedCanApplyRuntimeBuff;

        private bool isDesireCustomRotation = false;
        private bool reachedDesireCustomRotation = false;
        private Quaternion desiredRotation;

        public event Action EnteredReturnToPositionBeforeCombatState;
        
        public override AiStateTypeEnum EAiStateType => AiStateTypeEnum.Custom;

        public ReturnToPositionBeforeCombatState(NpcAI npcAI, Animator animator, NPCHealth npcHealth, BuffsController buffsController, AIPath aiPath) : base(npcAI)
        {
            _npcAI = npcAI;
            _animator = animator;
            _npcHealth = npcHealth;
            _buffsController = buffsController;
            _aiPath = aiPath;
        }

        public override void TickState()
        {
            base.TickState();

            if (_aiPath.reachedDestination && isDesireCustomRotation)
            {
                _npcAI.INpcBaseCreature.GetRootObjectTransform().rotation = Quaternion.RotateTowards(_npcAI.INpcBaseCreature.GetRootObjectTransform().rotation, desiredRotation, _aiPath.rotationSpeed * Time.deltaTime);
                
                reachedDesireCustomRotation = _npcAI.INpcBaseCreature.GetRootObjectTransform().rotation.AlmostEqualTo(desiredRotation);
            }
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);
            
            EnteredReturnToPositionBeforeCombatState?.Invoke();

            isDesireCustomRotation = false;
            reachedDesireCustomRotation = false;

            if (previousState is BaseCombatState baseCombatState)
            {
                isDesireCustomRotation = true;

                _aiPath.destination = baseCombatState.CachedOnEnterPosition;
                desiredRotation = baseCombatState.CachedOnEnterRotation;
            }

            _npcHealth.RestoreFullHP(); // restore full hp
            
            cachedHealthIsInvulnerable = _npcHealth.IsInvulnerable;
            _npcHealth.IsInvulnerable = true;

            cachedHealthInvulnerabilityType = _npcHealth.InvulnerabilityType;
            _npcHealth.InvulnerabilityType = BaseHealth.ImmortalityTypeEnum.NoDamage;

            cachedCanEngage = _npcAI.INpcBaseCreature.CombatInfoHandler.CanEngage;
            _npcAI.INpcBaseCreature.CombatInfoHandler.CanEngage = false;

            cachedCanApplyRuntimeBuff = _buffsController.CanApplyRuntimeBuff;
            _buffsController.CanApplyRuntimeBuff = false;
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _npcHealth.IsInvulnerable = cachedHealthIsInvulnerable;
            _npcHealth.InvulnerabilityType = cachedHealthInvulnerabilityType;
            _npcAI.INpcBaseCreature.CombatInfoHandler.CanEngage = cachedCanEngage;
            _buffsController.CanApplyRuntimeBuff = cachedCanApplyRuntimeBuff;
        }

        public Func<bool> CanSwitchToIdle() => () => { return _aiPath.reachedDestination && reachedDesireCustomRotation; };
    }
}