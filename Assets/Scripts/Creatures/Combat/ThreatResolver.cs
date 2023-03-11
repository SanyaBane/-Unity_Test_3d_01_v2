using System;
using Assets.Scripts.Creatures.Combat;

namespace Assets.Scripts.Creatures.Combat
{
    public class ThreatResolver
    {
        private readonly CombatInfoHandler _combatInfoHandler;
        
        public ThreatResolver(CombatInfoHandler combatInfoHandler)
        {
            _combatInfoHandler = combatInfoHandler;
            
            combatInfoHandler.CombatInfosAdded += CombatInfoHandlerOnCombatInfosAdded;
            combatInfoHandler.CombatInfosRemoved += CombatInfoHandlerOnCombatInfosRemoved;
        }

        public event Action<SingleCreatureCombatData> IncomeThreatChanged;
        public event Action<SingleCreatureCombatData> OutcomeThreatChanged;

        private void CombatInfoHandlerOnCombatInfosRemoved(CombatInfo combatInfo)
        {
            combatInfo.GetSingleCreatureCombatData(_combatInfoHandler.IBaseCreature).ThreatChanged -= OnIncomeThreatChanged;
            // combatInfo.GetCreatureCombatInfoData(_combatInfoHandler._baseCreature).ThreatChanged -= OnOutcomeThreatChanged;
        }
        
        private void CombatInfoHandlerOnCombatInfosAdded(CombatInfo combatInfo)
        {
            combatInfo.GetSecondSingleCreatureCombatData(_combatInfoHandler.IBaseCreature).ThreatChanged += OnIncomeThreatChanged;
            // combatInfo.GetCreatureCombatInfoData(_combatInfoHandler._baseCreature).ThreatChanged += OnOutcomeThreatChanged;
        }

        private void OnIncomeThreatChanged(SingleCreatureCombatData singleCreatureCombatData)
        {
            // Debug.Log($"{_combatInfoHandler._baseCreature.GetRootObjectTransform().gameObject.name}: OnThreatChanged. {singleCreatureCombatData.BaseCreature.GetRootObjectTransform().gameObject.name}: {singleCreatureCombatData.Threat}");
            IncomeThreatChanged?.Invoke(singleCreatureCombatData);
        }
        
        private void OnOutcomeThreatChanged(SingleCreatureCombatData singleCreatureCombatData)
        {
            // Debug.Log($"{_combatInfoHandler._baseCreature.GetRootObjectTransform().gameObject.name}: OnThreatChanged. {singleCreatureCombatData.BaseCreature.GetRootObjectTransform().gameObject.name}: {singleCreatureCombatData.Threat}");
            OutcomeThreatChanged?.Invoke(singleCreatureCombatData);
        }

        public CombatInfo GetCombatInfoWithMaxInputThreat()
        {
            CombatInfo ret = null;

            int creatureWithMaxThreat = -1;
            foreach (var engagedCreature in _combatInfoHandler.GetEngagedCreatures())
            {
                var combatInfo = engagedCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(_combatInfoHandler.IBaseCreature);
                int threatToCreature = combatInfo.GetThreatToCreature(_combatInfoHandler.IBaseCreature);
                if (threatToCreature > creatureWithMaxThreat)
                {
                    ret = combatInfo;
                    creatureWithMaxThreat = threatToCreature;
                }
            }

            return ret;
        }
    }
}