#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    [CustomEditor(typeof(NpcAI), true)]
    public class NpcAIEditor : Editor
    {
        private void OnSceneGUI()
        {
            NpcAI npcAI = target as NpcAI;

            if (npcAI.DrawEngageDistance)
            {
                Handles.color = new Color(1, 0.92f, 0.01f);
                Handles.DrawWireArc(npcAI.transform.position, Vector3.up, Vector3.forward, 360, npcAI.GetCompleteEngageDistance());
            }

            if (npcAI.DrawDisengageDistance)
            {
                Handles.color = new Color(1, 0.72f, 0.01f);
                Handles.DrawWireArc(npcAI.transform.position, Vector3.up, Vector3.forward, 360, npcAI.GetCompleteDisengageDistance());
            }
            
            // Handles.SphereHandleCap();
        }
    }
}
#endif