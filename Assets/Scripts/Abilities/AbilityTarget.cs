using System.Collections.Generic;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Factions;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public class AbilityTarget : Ability
    {
        public AbilityTargetSO AbilityTargetSO => (AbilityTargetSO) AbilitySO;
        
        public bool IsLookOnTarget;
        public float Distance;
        public bool TargetIsAlwaysSelf;
        public EAbilityAffects CanCastOn;
        public List<GameObject> CollisionParticlePrefabs;

        public AbilityTarget(AbilityTargetSO abilityTargetSO, IAbilitiesController iAbilitiesController) : base(abilityTargetSO, iAbilitiesController)
        {
            IsLookOnTarget = abilityTargetSO.IsLookOnTarget;
            Distance = abilityTargetSO.Distance;
            TargetIsAlwaysSelf = abilityTargetSO.TargetIsAlwaysSelf;
            CanCastOn = abilityTargetSO.CanCastOn;
            CollisionParticlePrefabs = abilityTargetSO.CollisionParticlePrefabs;
        }

        public override bool IsAbilityCanStartOrFinishCast(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            // if (base.IsAbilityCanStartOrFinishCast(abilityParameters) == false)
            //     return false;

            if (!TargetIsAlwaysSelf)
            {
                if (iAbilityParameters.DefaultAbilityParameters.Target == null || iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() == null)
                {
                    if (displayErrorMessage)
                        DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_NEED_TARGET, iAbilityParameters);

                    return false;
                }


                if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature == iAbilityParameters.DefaultAbilityParameters.Source)
                {
                    if (CanCastOn.HasFlag(EAbilityAffects.Self) == false)
                    {
                        if (displayErrorMessage)
                            DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_ON_SELF, iAbilityParameters);

                        return false;
                    }
                }
                else
                {
                    var relationWithSelectedTarget = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Faction.GetRelationWith(iAbilityParameters.DefaultAbilityParameters.Source.Faction);

                    if (relationWithSelectedTarget <= EFactionRelation.Neutral &&
                        CanCastOn.HasFlag(EAbilityAffects.Enemies) == false)
                    {
                        if (displayErrorMessage)
                            DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_ON_ENEMY, iAbilityParameters);

                        return false;
                    }

                    if (relationWithSelectedTarget > EFactionRelation.Neutral &&
                        CanCastOn.HasFlag(EAbilityAffects.Allies) == false)
                    {
                        if (displayErrorMessage)
                            DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_ON_ALLY, iAbilityParameters);

                        return false;
                    }
                }

                bool targetNotBlockedByTerrain = TargetHelper.IsNotBlockedByTerrain(iAbilityParameters.DefaultAbilityParameters.Source, iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature, isDrawRays: false);
                if (!targetNotBlockedByTerrain)
                {
                    if (displayErrorMessage)
                        DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_TARGET_IS_NOT_SEEN, iAbilityParameters);

                    return false;
                }

                // if required distance is "0", skip distance check
                if (Distance > 0)
                {
                    float distanceToTarget = TargetHelper.DistanceBetweenCreatureColliders(iAbilityParameters.DefaultAbilityParameters.Source, iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature);
                    if (distanceToTarget > Distance)
                    {
                        if (displayErrorMessage)
                            DisplayErrorMessage(Constants.MESSAGE_CANT_CAST_TARGET_TOO_FAR_AWAY, iAbilityParameters);

                        return false;
                    }
                }
            }

            return true;
        }

        public override AbilityParameters CreateAbilityParameters(IBaseCreature iBaseCreature)
        {
            var target = iBaseCreature.ICanSelectTarget.SelectedTarget;

            // if selected target is "Enemy" and ability can not be casted on "Enemy", but can be casted on "Self", then switch target to "Self"
            if (iBaseCreature.ICanSelectTarget.SelectedTarget == null)
            {
                if (!CanCastOn.HasFlag(EAbilityAffects.Enemies) && CanCastOn.HasFlag(EAbilityAffects.Self))
                {
                    target = iBaseCreature.ITargetable;
                }
            }
            else
            {
                var relationWithSelectedTarget = iBaseCreature.Faction.GetRelationWith(iBaseCreature.ICanSelectTarget.SelectedTarget.IBaseCreature.Faction);
                if (relationWithSelectedTarget <= EFactionRelation.Neutral)
                {
                    if (!CanCastOn.HasFlag(EAbilityAffects.Enemies) && CanCastOn.HasFlag(EAbilityAffects.Self))
                    {
                        target = iBaseCreature.ITargetable;
                    }
                }
            }

            var abilityParameters = new AbilityParameters()
            {
                DefaultAbilityParameters = new DefaultAbilityParameters(iBaseCreature, target)
            };

            return abilityParameters;
        }

        public override void ExecuteAbility(IAbilityParameters iAbilityParameters)
        {
            ApplyAbilityBehaviours(iAbilityParameters);
        }

        public override bool IsAbilityCanStartCast(IAbilityParameters abilityParameters, bool displayErrorMessage)
        {
            if (base.IsAbilityCanStartCast(abilityParameters, displayErrorMessage) == false)
                return false;

            if (abilityParameters.DefaultAbilityParameters.Target.CanBeAttacked == false)
                return false;

            return true;
        }
    }
}