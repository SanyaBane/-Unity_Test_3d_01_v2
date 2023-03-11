using Assets.Scripts.Interfaces;

namespace Assets.Scripts.TargetHandling
{
    public class NpcTargetHandler : TargetHandler
    {
        protected INpcBaseCreature _npcBaseCreature;

        protected override void Start()
        {
            base.Start();

            _npcBaseCreature = (INpcBaseCreature)_baseCreature;
            //_npcBaseCreature = this.GetComponent<INPCBaseCreature>();
        }
    }
}
