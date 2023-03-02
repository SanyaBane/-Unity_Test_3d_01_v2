using Assets.Scripts.NPC;
using Assets.Scripts.NPC.Tactics;
using UnityEngine;

namespace Assets.Scripts.Levels
{
    public class BaseLevelManager : MonoBehaviour
    {
        public virtual BaseAICombatTactics GetCombatTactics(NpcAI npcAI)
        {
            var result = new CombatTacticsAutoAttack(npcAI);
            return result;
        }

        public virtual BaseAIPeaceTactics GetPeaceTactics(NpcAI npcAI)
        {
            var result = new PeaceTacticsDoNothing(npcAI);
            return result;
        }
    }
}