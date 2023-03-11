using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Party
{
    public class PartyMemberData
    {
        public PartyMemberData(IBaseCreature iBaseCreature, ERole role)
        {
            IBaseCreature = iBaseCreature;
        }
        
        public IBaseCreature IBaseCreature { get; set; }
    }
}