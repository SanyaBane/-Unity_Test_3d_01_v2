using System.Linq;
using Assets.Scripts.Abilities;
using Assets.Scripts.Creatures;
using Assets.Scripts.Factions;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.NPC.Tactics
{
    public abstract class BaseAICombatTactics
    {
        public NpcAI NpcAI { get; }

        public bool IsCombatTacticsHandleMoving { get; protected set; } = false; 
        public bool IsPrioritizeMovingOverDamageDealing { get; protected set; } = false;

        public virtual float PreferableAttackDistance => NpcAI.INpcBaseCreature.AutoAttackController.AbilityAutoAttack.PreferableMaxDistance;

        public ITargetable CurrentTarget { get; set; } = null;
        
        protected BaseAICombatTactics(NpcAI npcAI)
        {
            NpcAI = npcAI;
            
            NpcAI.INpcBaseCreature.AbilitiesController.CastTicked += AbilitiesControllerOnCastTicked;
            NpcAI.INpcBaseCreature.AbilitiesController.CastFinished += AbilitiesControllerOnCastFinished;
        }

        protected virtual void AbilitiesControllerOnCastTicked(AbilitiesController abilitiesController, Ability ability, float currentlyCastedTime)
        {
            if (IsPrioritizeMovingOverDamageDealing && !ability.IsCastInstant(NpcAI.INpcBaseCreature))
                abilitiesController.InterruptCast();
        }
        
        protected virtual void AbilitiesControllerOnCastFinished(AbilitiesController abilitiesController, Ability ability)
        {
        }

        public abstract void ProcessCombatTactics();

        public virtual void ResetCombatTactics()
        {
            
        }
        
        public void AttemptToFindEnemy()
        {
            if (!NpcAI.INpcBaseCreature.CombatInfoHandler.GetCombatInfos().Any())
            {
                if (NpcAI.CanLookForEnemies)
                {
                    var foundedNearestEnemy = CheckAroundForEnemy();
                    if (foundedNearestEnemy != null)
                    {
                        bool alreadyEngagedWithCreature = NpcAI.INpcBaseCreature.CombatInfoHandler.IsAlreadyEngagedWithCreature(foundedNearestEnemy);
                        if (!alreadyEngagedWithCreature)
                        {
                            NpcAI.INpcBaseCreature.CombatInfoHandler.EngageCombat(foundedNearestEnemy);
                        }
                    }
                }
            }
        }
        
        private IBaseCreature CheckAroundForEnemy()
        {
            // возможно EngageDistance нужно будет менять с учётом радиуса коллайдера существа (просто прибавлять радиус из CreatureMeasures к engageDistance)
            var creaturesInAgroDistance = Physics.OverlapSphere(NpcAI.INpcBaseCreature.GetRootObjectTransform().position, NpcAI.GetCompleteEngageDistance(), LayerMask.GetMask(LayerManager.LAYER_NAME_CREATURE));

            IBaseCreature nearestEnemy = null;
            float distanceToNearestEnemy = 0;
            foreach (var creatureCollider in creaturesInAgroDistance)
            {
                var creatureLayerInfo = CreatureHelper.GetCreatureLayerInfoInfoFromCreatureLayerObject(creatureCollider.transform);
                var baseCreature = creatureLayerInfo.IBaseCreature;
                if (baseCreature == null)
                {
                    Debug.LogError($"{nameof(baseCreature)} == null");
                    continue;
                }

                if (baseCreature == NpcAI.INpcBaseCreature || //exclude creature itself
                    !baseCreature.Health.IsAlive //exclude others Dead creatures
                )
                {
                    continue;
                }

                if (baseCreature.Faction == null)
                {
                    Debug.LogError($"{nameof(baseCreature.Faction)} == null");
                    continue;
                }

                EFactionRelation relationToAgroDistanceCreature = NpcAI.INpcBaseCreature.Faction.GetRelationWith(baseCreature.Faction);

                if (relationToAgroDistanceCreature == EFactionRelation.Enemy)
                {
                    float distanceToCreature = TargetHelper.DistanceBetweenCreatureColliders(NpcAI.INpcBaseCreature, baseCreature);

                    if (nearestEnemy == null ||
                        distanceToCreature < distanceToNearestEnemy)
                    {
                        bool isNotBlockedByTerrain = TargetHelper.IsNotBlockedByTerrain(NpcAI.INpcBaseCreature, baseCreature, isDrawRays: false);
                        if (isNotBlockedByTerrain)
                        {
                            nearestEnemy = baseCreature;
                            distanceToNearestEnemy = distanceToCreature;
                        }
                    }
                    else
                    {
                        // this creature is farther then nearestEnemy, so do nothing
                    }
                }
            }

            return nearestEnemy;
        }
    }
}