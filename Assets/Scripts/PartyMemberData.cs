using Assets.Scripts.Interfaces;

namespace Assets.Scripts
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