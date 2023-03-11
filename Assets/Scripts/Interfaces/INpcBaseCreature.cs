using Assets.Scripts.Creatures;
using Assets.Scripts.NPC;
using Assets.Scripts.TargetHandling;

namespace Assets.Scripts.Interfaces
{
    public interface INpcBaseCreature : IBaseCreature
    {
        NpcTargetHandler NPCTargetHandler { get; }
        NpcAI NpcAI { get; }
        DebugNpcBaseCreature DebugNpcBaseCreature { get; }
        
        //float AgroDistance { get; }
        //bool IsDisengageOnDistance { get; }
        //float DisengageDistance { get; }
    }
}
