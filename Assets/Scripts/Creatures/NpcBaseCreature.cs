using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.NPC;
using Assets.Scripts.TargetHandling;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Creatures
{
    public class NpcBaseCreature : BaseCreature, INpcBaseCreature
    {
        public NpcTargetHandler NPCTargetHandler =>  (NpcTargetHandler) TargetHandler;

        private NpcAI _npcAI;
        public NpcAI NpcAI =>  _npcAI;

        [SerializeField] private DebugNpcBaseCreature _debugNpcBaseCreature;
        public DebugNpcBaseCreature DebugNpcBaseCreature => _debugNpcBaseCreature;

        protected override void Awake()
        {
            base.Awake();
            
            _npcAI = GetComponent<NpcAI>();
            _debugNpcBaseCreature = GetComponent<DebugNpcBaseCreature>();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}