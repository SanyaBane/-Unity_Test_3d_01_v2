using Assets.Scripts.Factions;
using Assets.Scripts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.NPC;
using Assets.Scripts.TargetHandling;
using UnityEngine;

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
