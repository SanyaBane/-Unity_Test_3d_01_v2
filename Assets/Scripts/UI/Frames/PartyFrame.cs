using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PartyFrame : MonoBehaviour
    {
        public PlayerFrame PlayerFrame;
        public GameObject PartyMemberUIFramePrefab;
        public Transform PartyMembersContainer;

        private readonly List<PlayerFrame> AllPartyMembersFrames = new List<PlayerFrame>();
        private readonly List<PlayerFrame> PartyMembersFrames = new List<PlayerFrame>();

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            var playerCreature = GameManager.Instance.PlayerCreature;
            
            var playerParty = playerCreature.PartyController.CurrentParty;
            if (playerParty != null)
            {
                PartyControllerOnCurrentPartyChanged(playerParty);
            }
            
            playerCreature.PartyController.CurrentPartyChanged += PartyControllerOnCurrentPartyChanged;
        }

        private void PartyControllerOnCurrentPartyChanged(PartyEntity partyEntity)
        {
            AllPartyMembersFrames.Clear();
            AllPartyMembersFrames.Add(PlayerFrame);
            
            PartyMembersFrames.Clear();
            
            foreach (Transform child in PartyMembersContainer) 
            {
                Destroy(child.gameObject);
            }

            if (partyEntity != null)
            {
                FillPartyMembersFrames(partyEntity);
            }
        }

        private void FillPartyMembersFrames(PartyEntity partyEntity)
        {
            var playerCreature = GameManager.Instance.PlayerCreature;

            var partyMemberWithoutPlayer = partyEntity.GetAllPartyMembers().Where(x => x != playerCreature).ToArray();
            foreach (var baseCreature in partyMemberWithoutPlayer)
            {
                PartyEntity_OnPartyMemberAdded(baseCreature);
            }
            
            partyEntity.PartyMemberAdded += PartyEntity_OnPartyMemberAdded;
            partyEntity.PartyMemberRemoved += PartyEntity_OnPartyMemberRemoved;
        }

        private void PartyEntity_OnPartyMemberAdded(IBaseCreature newPartyMemberBaseCreature)
        {
            var partyMemberFrameGO = Instantiate(PartyMemberUIFramePrefab, PartyMembersContainer);
            var partyMemberFrame = partyMemberFrameGO.GetComponent<PlayerFrame>();

            partyMemberFrame.Init();
            partyMemberFrame.SetNewOwner(newPartyMemberBaseCreature);
            partyMemberFrame.PartyFrame = this;

            PartyMembersFrames.Add(partyMemberFrame);
        }

        private void PartyEntity_OnPartyMemberRemoved(IBaseCreature obj)
        {
            throw new System.NotImplementedException();
        }
        
        public int GetMaxThreatInsidePartyForCreature(IBaseCreature target)
        {
            int maxThreat = 0;

            foreach (var playerFrame in AllPartyMembersFrames)
            {
                var combatInfo = playerFrame.CurrentFrameOwner.CombatInfoHandler.GetCombatInfoBySecondCreature(target);

                if (combatInfo == null)
                    continue;

                var threat = combatInfo.GetThreatFromCreature(playerFrame.CurrentFrameOwner);
                if (threat > maxThreat)
                    maxThreat = threat;
            }

            return maxThreat;
        }
    }
}