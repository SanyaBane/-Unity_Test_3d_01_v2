using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Creatures
{
    public class CombatInfoHandler
    {
        private readonly List<CombatInfo> _combatInfos = new List<CombatInfo>();
        
        public CombatInfoHandler(IBaseCreature baseCreature)
        {
            IBaseCreature = baseCreature;

            ThreatResolver = new ThreatResolver(this);
        }
        
        public IBaseCreature IBaseCreature { get; }
        public ThreatResolver ThreatResolver { get; }

        // private readonly List<ITargetable> _targetsToAttack = new List<ITargetable>();

        public int CombatInfosCount => _combatInfos.Count;
        public bool IsInCombat => _combatInfos.Count > 0;
        
        public bool CanEngage { get; set; } = true;
        
        // public event Action<ITargetable> TargetsToAttackAdded;
        
        public event Action<CombatInfo> CombatInfosAdded;
        public event Action<CombatInfo> CombatInfosRemoved;
        
        
        public IEnumerable<CombatInfo> GetCombatInfos()
        {
            foreach (var element in _combatInfos)
                yield return element;
        }
        
        // public IEnumerable<ITargetable> GetTargetsToAttack()
        // {
        //     foreach (var element in _targetsToAttack)
        //         yield return element;
        // }

        public CombatInfo GetCombatInfoBySecondCreature(IBaseCreature secondCreature)
        {
            var ret = _combatInfos.FirstOrDefault(x => x.GetSecondCreature(IBaseCreature) == secondCreature);
            return ret;
        }

        public IEnumerable<IBaseCreature> GetEngagedCreatures()
        {
            foreach (var element in _combatInfos)
            {
                yield return element.GetSecondCreature(IBaseCreature);
            }
        }

        public void AddCombatInfo(CombatInfo combatInfo)
        {
            _combatInfos.Add(combatInfo);

            CombatInfosAdded?.Invoke(combatInfo);
        }
        
        // public void AddTargetToAttack(ITargetable iTargetable)
        // {
        //     _targetsToAttack.Add(iTargetable);
        //
        //     TargetsToAttackAdded?.Invoke(iTargetable);
        // }

        public void RemoveCombatInfo(CombatInfo combatInfo)
        {
            _combatInfos.Remove(combatInfo);

            CombatInfosRemoved?.Invoke(combatInfo);
        }

        public bool IsAlreadyEngagedWithCreature(IBaseCreature otherCreature)
        {
            if (otherCreature == IBaseCreature)
                throw new Exception("Can not engage to combat with self."); // может быть это должен быть не Exception, а обычный "return"?

            var combatInfoWithOtherCreature = _combatInfos.FirstOrDefault(x => x.SingleCreatureCombatData1.BaseCreature == otherCreature || x.SingleCreatureCombatData2.BaseCreature == otherCreature);
            return combatInfoWithOtherCreature != null;
        }

        public CombatInfo EngageCombat(IBaseCreature engageCreature)
        {
            if (CanEngage == false)
                return null;

            var combatInfoWithOtherCreature = _combatInfos.FirstOrDefault(x => x.SingleCreatureCombatData1.BaseCreature == engageCreature || x.SingleCreatureCombatData2.BaseCreature == engageCreature);
            if (combatInfoWithOtherCreature != null)
                return combatInfoWithOtherCreature;

            //Debug.Log($"Engaged battle between '{_baseCreature.GetTransform().name}' and '{engageCreature.GetTransform().name}'.");

            var newCombatInfo = new CombatInfo(IBaseCreature, engageCreature);
            newCombatInfo.Engage();

            return newCombatInfo;
        }

        public void DisengageCombat(IBaseCreature disengageCreature)
        {
            if (disengageCreature == IBaseCreature)
                throw new Exception("Can not disengage combat with self."); // может быть это должен быть не Exception, а обычный "return"?

            var alreadyExistedCombatInfo = _combatInfos.FirstOrDefault(x => x.SingleCreatureCombatData1.BaseCreature == disengageCreature || x.SingleCreatureCombatData2.BaseCreature == disengageCreature);
            if (alreadyExistedCombatInfo != null)
            {
                alreadyExistedCombatInfo.Disengage();
            }
        }

        public void DisengageCombatWithEveryone()
        {
            for (int i = _combatInfos.Count - 1; i >= 0; i--)
            {
                var combatInfo = _combatInfos[i];

                combatInfo.Disengage();
            }
        }
    }
}