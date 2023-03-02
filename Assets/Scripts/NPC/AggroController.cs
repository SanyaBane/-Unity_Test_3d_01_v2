using System.Linq;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class AggroController
    {
        private int lastFrameCachePrimaryTarget = -1;
        private IBaseCreature cachedPrimaryTarget = null;

        private readonly NpcAI _npcAI;
        
        public AggroController(NpcAI npcAI)
        {
            _npcAI = npcAI;
        }

        public IBaseCreature GetPrimaryTarget()
        {
            if (Time.frameCount == lastFrameCachePrimaryTarget)
                return cachedPrimaryTarget;
            
            var primaryTarget = GetPrimaryTargetPrivate();

            lastFrameCachePrimaryTarget = Time.frameCount;
            cachedPrimaryTarget = primaryTarget;
            
            return primaryTarget;
        }
        
        private IBaseCreature GetPrimaryTargetPrivate()
        {
            var provokeDebuffs = _npcAI.INpcBaseCreature.BuffsController.GetAllBuffs()
                .Where(x => x.BaseBuffSO.Id == ConstantsAbilities_JobTank.BUFF_TANK_PROVOKE).ToList();

            if (provokeDebuffs.Count > 0)
            {
                if (provokeDebuffs.Count == 1)
                {
                    return provokeDebuffs.First().Source;
                }
            
                var earliestProvokeDebuff = provokeDebuffs
                    .Aggregate((buff1, buff2) => buff2.CreationTime < buff1.CreationTime ? buff2 : buff1);
            
                return earliestProvokeDebuff.Source;
            }
            
            IBaseCreature creatureToSelect = null;
            
            int creatureWithMaxThreat = -1;
            var engagedCreatures = _npcAI.INpcBaseCreature.CombatInfoHandler.GetEngagedCreatures().ToList();
            foreach (var engagedCreature in engagedCreatures)
            {
                var canSelectCreature = _npcAI.NPCTargetHandler.CanSelect(engagedCreature.ITargetable);
                if (canSelectCreature)
                {
                    float distanceToTarget = TargetHelper.DistanceBetweenCreatureColliders(_npcAI.INpcBaseCreature, engagedCreature);

                    if (_npcAI.IsDisengageOnDistance && distanceToTarget + _npcAI.INpcBaseCreature.CreatureMeasures.Radius > _npcAI.GetCompleteDisengageDistance())
                    {
                        continue;
                    }

                    var combatInfo = engagedCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(_npcAI.INpcBaseCreature);
                    int threatToCreature = combatInfo.GetThreatToCreature(_npcAI.INpcBaseCreature);
                    if (threatToCreature > creatureWithMaxThreat)
                    {
                        creatureWithMaxThreat = threatToCreature;
                        creatureToSelect = engagedCreature;
                    }
                }
                else
                {
                    _npcAI.INpcBaseCreature.CombatInfoHandler.DisengageCombat(engagedCreature);
                }
            }

            return creatureToSelect;
        }
    }
}