using Assets.Scripts.Factions;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
