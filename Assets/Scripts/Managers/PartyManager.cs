using System.Linq;
using Assets.Scripts.Factions;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class PartyManager : MonoBehaviour
    {
        public void AttackTarget(ITargetable target)
        {
            var playerCreature = GameManager.Instance.PlayerCreature;

            if (playerCreature.PartyController.CurrentParty == null)
            {
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage("No party members.");
                return;
            }
            
            if (target == null)
            {
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage("No target selected.");
                return;
            }

            var relationWithSelectedTarget = playerCreature.Faction.GetRelationWith(target.IBaseCreature.Faction);
            if (relationWithSelectedTarget > EFactionRelation.Neutral)
            {
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage("Can not order to attack friendly unit.");
                return;
            }

            var partyMembers = playerCreature.PartyController.CurrentParty.GetAllPartyMembers();
            var partyMembersExceptPlayer = partyMembers.Where(x => !ReferenceEquals(playerCreature, x));
            foreach (var partyMember in partyMembersExceptPlayer)
            {
                if (partyMember is INpcBaseCreature npcBaseCreature)
                {
                    npcBaseCreature.NpcAI.MoveToAttackTarget(target.IBaseCreature);
                }
            }
        }

        public void FollowLeader()
        {
            var playerCreature = GameManager.Instance.PlayerCreature;
            
            if (playerCreature.PartyController.CurrentParty == null)
            {
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage("No party members.");
                return;
            }

            var partyMembers = playerCreature.PartyController.CurrentParty.GetAllPartyMembers();
            var partyMembersExceptPlayer = partyMembers.Where(x => !ReferenceEquals(playerCreature, x));
            foreach (var partyMember in partyMembersExceptPlayer)
            {
                if (partyMember is INpcBaseCreature npcBaseCreature)
                {
                    npcBaseCreature.NpcAI.FollowTarget(playerCreature);
                }
            }
        }
        
        public void WithdrawToLeader()
        {
            var playerCreature = GameManager.Instance.PlayerCreature;
            
            if (playerCreature.PartyController.CurrentParty == null)
            {
                GameManager.Instance.GUIManager.PlayerErrorMessageContainer.DisplayErrorMessage("No party members.");
                return;
            }

            var partyMembers = playerCreature.PartyController.CurrentParty.GetAllPartyMembers();
            var partyMembersExceptPlayer = partyMembers.Where(x => !ReferenceEquals(playerCreature, x));
            foreach (var partyMember in partyMembersExceptPlayer)
            {
                if (partyMember is INpcBaseCreature npcBaseCreature)
                {
                    npcBaseCreature.NpcAI.GoToTarget(playerCreature);
                }
            }
        }
    }
}