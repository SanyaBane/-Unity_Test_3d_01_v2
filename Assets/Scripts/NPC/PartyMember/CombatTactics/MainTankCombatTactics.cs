using System;
using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Extensions;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Interfaces;
using Assets.Scripts.NPC.Tactics;
using UnityEngine;

namespace Assets.Scripts.NPC.PartyMember.CombatTactics
{
    public class MainTankCombatTactics : BaseAICombatTactics
    {
        private const float MOVING_TARGET_TO_TANK_POSITION_CREATURE_ACCEPTABLE_OFFSET_POSITION = 0.1f;

        private const float MOVING_TARGET_TO_TANK_POSITION_TARGET_ACCEPTABLE_OFFSET_POSITION = 0.3f;
        //private const float TARGET_TO_DESTINATION_POSSIBLE_OFFSET_FOR_CORRECTING_CREATURE_POSITION = 2.0f;
        
        protected AbilityTarget _tankStanceAbility;
        protected Ability _tankRangeAbility;
        
        public string GetTankStanceAbilityId
        {
            get
            {
                // switch (NpcAI.INpcBaseCreature.CurrentJob)
                switch (NpcAI.Job)
                {
                    case EJob.WAR:
                        return ConstantsAbilities_WAR.ABILITY_WAR_DEFIANCE;
                    case EJob.PAL:
                        return ConstantsAbilities_PAL.ABILITY_PAL_SHIELD_OATH;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string GetTankStanceBuffId
        {
            get
            {
                // switch (NpcAI.INpcBaseCreature.CurrentJob)
                switch (NpcAI.Job)
                {
                    case EJob.WAR:
                        return ConstantsAbilities_WAR.BUFF_WAR_DEFIANCE;
                    case EJob.PAL:
                        return ConstantsAbilities_PAL.BUFF_PAL_SHIELD_OATH;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public string GetTankRangeAbilityId
        {
            get
            {
                // switch (NpcAI.INpcBaseCreature.CurrentJob)
                switch (NpcAI.Job)
                {
                    case EJob.WAR:
                        return ConstantsAbilities_WAR.ABILITY_WAR_TOMAHAWK;
                    case EJob.PAL:
                        return ConstantsAbilities_PAL.ABILITY_PAL_SHIELD_LOB;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private AbilityTarget _heavySwing;
        private AbilityTarget _maim;
        private AbilityTarget _stormsEye;
        private AbilityTarget _stormsPath;
        private AbilityTargetProjectile _tomahawk;

        public MainTankCombatTactics(NpcAI npcAI) : base(npcAI)
        {
            _tankStanceAbility = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(GetTankStanceAbilityId);
            _tankRangeAbility = NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(GetTankRangeAbilityId);
            
            _heavySwing = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_WAR.ABILITY_WAR_HEAVY_SWING);
            _maim = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_WAR.ABILITY_WAR_MAIM);
            _stormsEye = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_WAR.ABILITY_WAR_STORMS_EYE);
            _stormsPath = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_WAR.ABILITY_WAR_STORMS_PATH);
            _tomahawk = (AbilityTargetProjectile) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(ConstantsAbilities_WAR.ABILITY_WAR_TOMAHAWK);
        }

        public override void ProcessCombatTactics()
        {
            ExecuteCombatRotation();
        }

        protected void ExecuteCombatRotation()
        {
            var buffStormsEye = NpcAI.INpcBaseCreature.BuffsController.GetBuffById(ConstantsAbilities_WAR.BUFF_WAR_STORMS_EYE);

            bool shouldCastStormsEye = false;
            if (NpcAI.CanTryCast(_stormsEye) && _stormsEye.TimeUntilComboActionAvailable > 0)
            {
                if (buffStormsEye == null)
                {
                    shouldCastStormsEye = true;
                }
                else
                {
                    var buffStormsEyeBuffDurationDefault = (BuffDurationDefault) buffStormsEye.BuffDuration;
                    if (buffStormsEyeBuffDurationDefault.RemainingDuration <= buffStormsEyeBuffDurationDefault.InitialDuration)
                    {
                        shouldCastStormsEye = true;
                    }
                }
            }

            if (shouldCastStormsEye)
            {
                NpcAI.TryCastAbility(_stormsEye);
            }
            else if (NpcAI.CanTryCast(_stormsPath) && _stormsPath.TimeUntilComboActionAvailable > 0)
            {
                NpcAI.TryCastAbility(_stormsPath);
            }
            else if (NpcAI.CanTryCast(_maim) && _maim.TimeUntilComboActionAvailable > 0)
            {
                NpcAI.TryCastAbility(_maim);
            }
            else if (NpcAI.CanTryCast(_heavySwing))
            {
                NpcAI.TryCastAbility(_heavySwing);
            }
        }
        
        protected void TankTargetAtPosition(INpcBaseCreature target, Vector3 targetPositionDestination, Vector3 rotateBossTo)
        {
            bool isChangedAIPathDistances = false;

            float offsetPositionInUnits = GetOffsetPositionForCreatureWhileMovingThreatenedTargetToTankingPosition(target, targetPositionDestination);
            var distanceSquaredTargetAndTargetDestination = VectorHelper.DistanceSquared(target.GetGroundedPosition(), targetPositionDestination);
            bool isTargetOnPosition = distanceSquaredTargetAndTargetDestination < MOVING_TARGET_TO_TANK_POSITION_TARGET_ACCEPTABLE_OFFSET_POSITION * MOVING_TARGET_TO_TANK_POSITION_TARGET_ACCEPTABLE_OFFSET_POSITION;
            if (isTargetOnPosition)
            {
                Vector3 creatureOffSetPositionConsideringRotation = rotateBossTo * offsetPositionInUnits;

                var distanceSquaredCreatureAndNeededPosition = VectorHelper.DistanceSquared(NpcAI.INpcBaseCreature.GetGroundedPosition(), creatureOffSetPositionConsideringRotation);
                bool isCreatureOnPosition = distanceSquaredCreatureAndNeededPosition < MOVING_TARGET_TO_TANK_POSITION_CREATURE_ACCEPTABLE_OFFSET_POSITION * MOVING_TARGET_TO_TANK_POSITION_CREATURE_ACCEPTABLE_OFFSET_POSITION;
                if (isCreatureOnPosition)
                {
                    // if creature and target are already on needed positions, then do nothing
                    //Debug.Log("Creature and target are on positions");
                }
                else
                {
                    // if target is on position, just move creature to "creature position"
                    //Debug.Log("Target is on position");
                    NpcAI.AIPath.destination = targetPositionDestination + creatureOffSetPositionConsideringRotation;
                    NpcAI.AIPath.SetEndReachedDistance(0.05f);
                    NpcAI.AIPath.SetSlowdownDistance(0.3f);
                    isChangedAIPathDistances = true;
                }
            }
            else
            {
                // if target is not on position, then we need to move creature in such way, that target will follow it and in the end, will stop on "target position"
                //Debug.Log($"Target is NOT on position. {nameof(distanceSquaredTargetAndDestination)} = {distanceSquaredTargetAndDestination}");
                var directionFromTargetToTargetPosition = (targetPositionDestination - target.GetGroundedPosition()).normalized;

                float specialOffset = 0.1f;

                Vector3 tempPositionForCreature = targetPositionDestination + directionFromTargetToTargetPosition * (offsetPositionInUnits + specialOffset);
                // if (distanceSquaredTargetAndTargetDestination < TARGET_TO_DESTINATION_POSSIBLE_OFFSET_FOR_CORRECTING_CREATURE_POSITION * TARGET_TO_DESTINATION_POSSIBLE_OFFSET_FOR_CORRECTING_CREATURE_POSITION)
                //     tempPositionForCreature = target.GetGroundedPosition() + directionFromTargetToTargetPosition * (offsetPositionInUnits + specialOffset);
                // else
                //     tempPositionForCreature = targetPositionDestination + directionFromTargetToTargetPosition * (offsetPositionInUnits + specialOffset);

                NpcAI.AIPath.destination = tempPositionForCreature;
                NpcAI.AIPath.SetEndReachedDistance(0.05f);
                NpcAI.AIPath.SetSlowdownDistance(0.3f);
                isChangedAIPathDistances = true;
            }

            if (!isChangedAIPathDistances)
            {
                if (NpcAI.AIPath != null)
                {
                    NpcAI.AIPath.SetSlowdownDistance(NpcAI.DefaultSlowdownDistance);
                    NpcAI.AIPath.SetEndReachedDistance(NpcAI.DefaultEndReachedDistance);
                }
            }
        }
        
        private float GetOffsetPositionForCreatureWhileMovingThreatenedTargetToTankingPosition(INpcBaseCreature target, Vector3 targetPositionDestination)
        {
            //float targetSlowdownDistanceToTarget = target.NpcAI.GetCombatState().GetSlowdownDistanceToTarget(_partyMemberAI.INpcBaseCreature);
            float targetEndReachedDistanceToTarget = target.NpcAI.GetCombatState().GetEndReachedDistanceToTarget(NpcAI.INpcBaseCreature);

            //var distanceSquaredBetweenCreatureAndTarget = VectorHelper.DistanceSquared(_partyMemberAI.INpcBaseCreature.GetGroundedPosition(), target.GetGroundedPosition());
            //var distanceSquaredTargetAndNeededPosition = VectorHelper.DistanceSquared(target.GetGroundedPosition(), targetPositionDestination);
            //if (distanceSquaredBetweenCreatureAndTarget < )

            float offsetPositionInUnits = targetEndReachedDistanceToTarget;
            return offsetPositionInUnits;
        }
    }
}