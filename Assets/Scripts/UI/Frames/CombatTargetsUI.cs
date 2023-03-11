using Assets.Scripts.Creatures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CombatTargetsUI : BaseUIFrame
    {
        [SerializeField] private GameObject CombatTargetSinglePrefab;

        private readonly List<CombatTargetSingle> currentListCombatTargetSingle = new List<CombatTargetSingle>();

        private PlayerCreature PlayerCreature;
        private CombatInfoHandler CombatInfoHandler;

        public override void Setup()
        {
            PlayerCreature = GameManager.Instance.PlayerCreature;
            CombatInfoHandler = PlayerCreature.CombatInfoHandler;

            CombatInfoHandler.CombatInfosAdded += CombatInfoHandler_CombatInfosAdded;
            CombatInfoHandler.CombatInfosRemoved += CombatInfoHandler_CombatInfosRemoved;
        }

        private void CombatInfoHandler_CombatInfosAdded(CombatInfo combatInfo)
        {
            CreateCombatTargetUI(combatInfo);
        }

        private void CombatInfoHandler_CombatInfosRemoved(CombatInfo combatInfo)
        {
            RemoveCombatTargetUI(combatInfo);
        }

        private void CreateCombatTargetUI(CombatInfo combatInfo)
        {
            var combatTargetSingleGameObject = Instantiate(CombatTargetSinglePrefab, this.gameObject.transform);
            var combatTargetSingleScript = combatTargetSingleGameObject.GetComponent<CombatTargetSingle>();

            var engagedCreature = combatInfo.GetSecondCreature(PlayerCreature);
            combatTargetSingleScript.SetupCreature(engagedCreature);

            currentListCombatTargetSingle.Add(combatTargetSingleScript);
        }

        private void RemoveCombatTargetUI(CombatInfo combatInfo)
        {
            var engagedCreature = combatInfo.GetSecondCreature(PlayerCreature);

            var existed = currentListCombatTargetSingle.First(x => x.OwnerCreature == engagedCreature);
            currentListCombatTargetSingle.Remove(existed);

            Destroy(existed.gameObject);
        }
    }
}
