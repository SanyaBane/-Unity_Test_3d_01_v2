using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class GoToTargetState : FollowTargetState
    {
        public GoToTargetState(NpcAI npcAI, Animator animator) : base(npcAI, animator)
        { }
    }
}