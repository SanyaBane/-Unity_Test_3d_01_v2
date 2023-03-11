using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Party
{
    public class PartyEntity
    {
        public const int MaxPartyMembers = 8;

        private List<IBaseCreature> _partyMembers;
        public IEnumerable<IBaseCreature> GetAllPartyMembers()
        {
            var ret = _partyMembers.ToArray();
            return ret;
        }

        public event Action<IBaseCreature> PartyMemberAdded;
        public event Action<IBaseCreature> PartyMemberRemoved;
        
        public IBaseCreature PartyLeader { get; private set; }

        public PartyEntity(IBaseCreature partyLeader)
        {
            if (!CanBeAddedToParty(partyLeader))
                throw new Exception("!CanBeAddedToParty");
            
            PartyLeader = partyLeader;

            _partyMembers = new List<IBaseCreature>
            {
                PartyLeader
            };

            PartyLeader.PartyController.CurrentParty = this;
        }

        public void InviteToParty(IBaseCreature baseCreature)
        {
            if (!CanBeAddedToParty(baseCreature))
                return;
            
            _partyMembers.Add(baseCreature);
            baseCreature.PartyController.CurrentParty = this;
            
            PartyMemberAdded?.Invoke(baseCreature);
        }

        public void KickFromParty(IBaseCreature baseCreature)
        {
            throw new NotImplementedException();
            
            PartyMemberRemoved?.Invoke(baseCreature);
        }
        
        public static bool CanBeAddedToParty(IBaseCreature baseCreature)
        {
            if (!baseCreature.PartyController.CanBeInParty)
            {
                Debug.LogError($"{baseCreature.CreatureInfoContainer.gameObject.name} can not be in party.");
                return false;
            }
            
            if (baseCreature.PartyController.CurrentParty != null)
            {
                Debug.LogError($"{baseCreature.CreatureInfoContainer.gameObject.name} already in party.");
                return false;
            }

            return true;
        }

        // public static PartyEntity CreateParty()
        // {
        //     
        // }
    }
}