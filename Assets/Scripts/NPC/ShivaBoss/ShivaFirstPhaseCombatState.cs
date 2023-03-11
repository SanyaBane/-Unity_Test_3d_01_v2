using Abilities.Cooldown;
using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Interfaces;
using Assets.Scripts.NPC.States;
using Assets.Scripts.StateMachineScripts;
using UnityEngine;

namespace Assets.Scripts.NPC.ShivaBoss
{
    public class ShivaFirstPhaseCombatState : BaseCombatState
    {
        private ShivaAI _shivaAI;

        public bool CastedAbsoluteZero { get; private set; } = false;
        private AbilityAOEFromSelf _absoluteZero;
        private float _absoluteZeroCastTime = 3.0f; // 10

        public bool CastedMirrorMirror { get; private set; } = false;
        private AbilityPlacebo _abilityMirrorMirror;
        private float _mirrorMirrorCastTime = 1.0f; // 23

        public int NumberOfCastedBitingOrDriving { get; private set; } = 0;
        private AbilityAOEFromSelf _bitingFrost;
        private AbilityAOEFromSelf _drivingFrost;
        private float _bitingOrDrivingCastTime = 10.0f; // 33

        private bool _firstCastIsBitingFrost;

        public ShivaFirstPhaseCombatState(ShivaAI shivaAI, INpcBaseCreature npcBaseCreature, Animator animator,
            AbilitiesController abilitiesController, AutoAttackController autoAttack)
            : base(shivaAI, npcBaseCreature, animator, abilitiesController, autoAttack)
        {
            _shivaAI = shivaAI;

            _absoluteZero = (AbilityAOEFromSelf) _abilitiesController.GetAbilityById(ShivaAI.ABILITY_ID_SHIVA_ABSOLUTE_ZERO);
            _absoluteZero.CastTime = 2f;
            ((CooldownDefault)_absoluteZero.AbilityCooldown).DefaultCooldown = 3f;

            _abilityMirrorMirror = (AbilityPlacebo) _abilitiesController.GetAbilityById(ShivaAI.ABILITY_ID_SHIVA_MIRROR_MIRROR);
            _abilityMirrorMirror.CastTime = 1;

            _bitingFrost = (AbilityAOEFromSelf) _abilitiesController.GetAbilityById(ShivaAI.ABILITY_ID_SHIVA_BITING_FROST);
            _bitingFrost.CastTime = 2;

            _drivingFrost = (AbilityAOEFromSelf) _abilitiesController.GetAbilityById(ShivaAI.ABILITY_ID_SHIVA_DRIVING_FROST);
            _drivingFrost.CastTime = 2;
        }
        
        protected override void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
            base.AbilitiesControllerOnCastFinished(abilitiesController, ability);

            if (ability.AbilitySO.Id == ShivaAI.ABILITY_ID_SHIVA_BITING_FROST || ability.AbilitySO.Id == ShivaAI.ABILITY_ID_SHIVA_DRIVING_FROST)
            {
                NumberOfCastedBitingOrDriving++;
                
                if (NumberOfCastedBitingOrDriving == 1)
                {
                    _shivaAI.ShivaLevelManager.MirrorMirror3FirstCastBitingOrDriving(ability.AbilitySO); // mirrors cast biting / driving
                }
            }
        }

        protected override void AbilitiesControllerOnCastFinishedAndExecuted(AbilitiesController abilitiesController, Ability ability)
        {
            base.AbilitiesControllerOnCastFinishedAndExecuted(abilitiesController, ability);

            if (ability.AbilitySO.Id == ShivaAI.ABILITY_ID_SHIVA_MIRROR_MIRROR)
            {
                _shivaAI.ShivaLevelManager.MirrorMirror3Summon(); // summon mirrors
            }
        }

        protected override void ExecuteSpecificBehaviour()
        {
            base.ExecuteSpecificBehaviour();
            
            // ProductionBehaviour();

            // if (NumberOfCastedBitingOrDriving == 0 && TimeInThisState > 0 && _npcAI.CastingAbility == false)
            // {
            //     _firstCastIsBitingFrost = false;
            //     _npcAI.TryCastAbility(_drivingFrost);
            // }
            
            // if (NumberOfCastedBitingOrDriving == 0 && TimeInThisState > 4 && _npcAI.CastingAbility == false)
            // {
            //     _firstCastIsBitingFrost = true;
            //     _npcAI.TryCastAbility(_bitingFrost);
            // }
            
            // if (!CastedMirrorMirror && TimeInThisState > 0 && _npcAI.CastingAbility == false)
            // {
            //     CastedMirrorMirror = true;
            //     _npcAI.TryCastAbility(_abilityMirrorMirror);
            // }
            //
            // if (NumberOfCastedBitingOrDriving == 0 && TimeInThisState > 3 && _npcAI.CastingAbility == false && CastedMirrorMirror)
            // {
            //     NumberOfCastedBitingOrDriving++;
            //
            //     // int rand = Random.Range(1, 3);
            //     // if (rand == 1)
            //     // {
            //         _firstCastIsBitingFrost = true;
            //         _npcAI.TryCastAbility(_bitingFrost);
            //     // }
            //     // else
            //     // {
            //     // _firstCastIsBitingFrost = false;
            //     // _npcAI.TryCastAbility(_drivingFrost);
            //     // }
            // }
        }

        private void ProductionBehaviour()
        {
            // _shivaAI.ShivaLevelManager.FightPhase == enFightPhase.StartFight
            if (!CastedAbsoluteZero && TimeInThisState > _absoluteZeroCastTime && _npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility == false)
            {
                CastedAbsoluteZero = true;
                _npcAI.TryCastAbility(_absoluteZero);
            }

            if (!CastedMirrorMirror && TimeInThisState > _mirrorMirrorCastTime && _npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility == false)
            {
                CastedMirrorMirror = true;
                _npcAI.TryCastAbility(_abilityMirrorMirror);
            }

            if (NumberOfCastedBitingOrDriving == 0 && TimeInThisState > _bitingOrDrivingCastTime && _npcAI.INpcBaseCreature.AbilitiesController.IsCastingOrFinishingCastingAbility == false)
            {
                int rand = Random.Range(1, 3);
                if (rand == 1)
                {
                    _firstCastIsBitingFrost = true;
                    _npcAI.TryCastAbility(_bitingFrost);
                }
                else
                {
                    _firstCastIsBitingFrost = false;
                    _npcAI.TryCastAbility(_drivingFrost);
                }
            }
        }

        public override void OnEnterState(IState previousState)
        {
            base.OnEnterState(previousState);

            _shivaAI.RaiseEnteredFirstPhase();

            // _absoluteZero.SetOnSpecificCooldown(1.0f);
            // _bitingFrost.SetOnSpecificCooldown(1.0f);
        }
    }
}