using System.Collections.Generic;
using Assets.Scripts.Abilities.Enums;
using Assets.Scripts.Abilities.General.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Factions;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Abilities.General
{
    public class AbilityAOEFromSelf : Ability
    {
        // public new AbilityAoeFromSelfSO AbilitySO => (AbilityAoeFromSelfSO) base.AbilitySO;
        private AbilityAOEFromSelfSO AbilityAoeFromSelfSO;

        public bool DisplayAoeZone{ get; set; }

        public EAbilityAffects AbilityAffects { get; set; }

        public float Height { get; set; }
        public float Radius { get; set; }
        public float Angle { get; set; }
        public float ClockwiseRotation { get; set; }

        public AbilityAOEFromSelf(AbilityAOEFromSelfSO abilityAoeFromSelfSO, IAbilitiesController iAbilitiesController) : base(abilityAoeFromSelfSO, iAbilitiesController)
        {
            AbilityAoeFromSelfSO = abilityAoeFromSelfSO;

            DisplayAoeZone = abilityAoeFromSelfSO.DisplayAOEZone;
            AbilityAffects = abilityAoeFromSelfSO.AbilityAffects;
            Height = abilityAoeFromSelfSO.Height;
            Radius = abilityAoeFromSelfSO.Radius;
            Angle = abilityAoeFromSelfSO.Angle;
            ClockwiseRotation = abilityAoeFromSelfSO.ClockwiseRotation;
        }

        public override IAbilityParameters CreateSpecialAbilityParameters(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var sourceCreaturePosition = iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform().position;
            // var creaturesInRadius = Physics.OverlapSphere(sourcePos, Radius, LayerMask.GetMask(LayerManager.LAYER_NAME_CREATURE));
            var creaturesInRadius = Physics.OverlapCapsule(sourceCreaturePosition + new Vector3(0, Height / 2, 0), sourceCreaturePosition - new Vector3(0, Height / 2, 0),
                Radius, LayerMask.GetMask(LayerManager.LAYER_NAME_CREATURE));

            var targets = new List<IBaseCreature>();

            foreach (var creatureCollider in creaturesInRadius)
            {
                var creatureLayerInfo = CreatureHelper.GetCreatureLayerInfoInfoFromCreatureLayerObject(creatureCollider.transform);
                var baseCreature = creatureLayerInfo.IBaseCreature;
                if (baseCreature == null)
                {
                    Debug.LogError($"{nameof(baseCreature)} == null");
                    continue;
                }

                if (!AbilityAffects.HasFlag(EAbilityAffects.Self) && iAbilityParameters.DefaultAbilityParameters.Source == baseCreature)
                {
                    continue;
                }

                if (baseCreature.Faction == null)
                {
                    Debug.LogError($"{nameof(baseCreature.Faction)} == null");
                    continue;
                }


                if (baseCreature.ITargetable.CatchAOEByCapsuleCollider == false)
                {
                    // Some creatures (mostly players) has slightly different AOE checks. While usual creatures should be able to "catch" AOE by their "Measure" Radius,
                    // the only way for player to catch AOE is to directly being inside of it (imagine that their allowed Radius is 0.001).

                    Vector3 sourceCreatureVerticalPosition = new Vector3(0, sourceCreaturePosition.y, 0);
                    Vector3 dstCreatureVerticalPosition = new Vector3(0, creatureCollider.transform.position.y, 0);
                    var verticalDistanceSquared = VectorHelper.DistanceSquared(sourceCreatureVerticalPosition, dstCreatureVerticalPosition);

                    float fixMeasurementErrorValue = 0.01f; // // some value to make sure float calculations won't intervene our logic
                    if (baseCreature is PlayerCreature playerCreature)
                    {
                        fixMeasurementErrorValue += playerCreature.IPlayerController.CharacterController.skinWidth;
                    }

                    float comparisonValue = Mathf.Pow(Height / 2 + fixMeasurementErrorValue, 2);
                    if (verticalDistanceSquared > comparisonValue)
                    {
                        continue; // let's try to comment this section and for now check only inside of "Height" (because using "OverlapCapsule" we also check above and bellow Height using spheres)

                        // var distanceToPlayerCreatureSquared = VectorHelper.DistanceSquared(sourceCreaturePosition, creatureCollider.transform.position);
                        //
                        // // check Top/Bottom of capsule
                        // if (distanceToPlayerCreatureSquared > Mathf.Pow(Radius, 2))
                        //     continue;
                    }
                    else
                    {
                        Vector3 sourceCreatureHorizontalPosition = new Vector3(sourceCreaturePosition.x, 0, sourceCreaturePosition.z);
                        Vector3 dstCreatureHorizontalPosition = new Vector3(creatureCollider.transform.position.x, 0, creatureCollider.transform.position.z);
                        var horizontalDistanceSquared = VectorHelper.DistanceSquared(sourceCreatureHorizontalPosition, dstCreatureHorizontalPosition);

                        // check Center of capsule
                        if (horizontalDistanceSquared > Mathf.Pow(Radius, 2))
                            continue;
                    }
                }

                EFactionRelation relationToAgroDistanceCreature = iAbilityParameters.DefaultAbilityParameters.Source.Faction.GetRelationWith(baseCreature.Faction);

                if (!AbilityAffects.HasFlag(EAbilityAffects.Allies) && relationToAgroDistanceCreature > EFactionRelation.Neutral)
                {
                    continue;
                }

                if (!AbilityAffects.HasFlag(EAbilityAffects.Enemies) && relationToAgroDistanceCreature <= EFactionRelation.Neutral)
                {
                    continue;
                }

                var directionToTarget = (baseCreature.GetRootObjectTransform().position - sourceCreaturePosition).normalized;
                directionToTarget = new Vector3(directionToTarget.x, 0, directionToTarget.z); // reset Y axis

                var forwardVectorClockwiseRotated = Quaternion.Euler(0, ClockwiseRotation, 0) * iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform().forward;
                forwardVectorClockwiseRotated = new Vector3(forwardVectorClockwiseRotated.x, 0, forwardVectorClockwiseRotated.z); // reset Y axis

                float dotProduct = Vector3.Dot(directionToTarget, forwardVectorClockwiseRotated);
                float dotProductDegrees = Mathf.Acos(dotProduct) * 2 * Mathf.Rad2Deg;
                if (dotProductDegrees > Angle)
                {
                    continue;
                }

                if (!baseCreature.Health.IsAlive) //exclude others Dead creatures
                {
                    continue;
                }

                if (relationToAgroDistanceCreature > EFactionRelation.Neutral)
                {
                    continue;
                }

                targets.Add(baseCreature);
            }

            var specialAbilityParameters = new AbilityParametersAOE()
            {
                DefaultAbilityParameters = iAbilityParameters.DefaultAbilityParameters,
                Targets = targets
            };

            return specialAbilityParameters;
        }

        public override void ExecuteAbility(IAbilityParameters iAbilityParameters)
        {
            ApplyAbilityBehaviours(iAbilityParameters);
        }

        public override bool IsAbilityCanStartOrFinishCast(IAbilityParameters iAbilityParameters, bool displayErrorMessage)
        {
            return true;
        }

        public override void OnCastFinished(IAbilityParameters iAbilityParameters)
        {
            foreach (var particle in AbilityAoeFromSelfSO.ParticlesCasterCastFinish)
            {
                GameObject.Instantiate(particle, iAbilityParameters.DefaultAbilityParameters.Source.AttachmentsController.Attach_Hitloc.position, Quaternion.identity);
            }
        }
    }
}