#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [CustomEditor(typeof(CreatureMeasures))]
    public class CreatureMeasuresEditor : Editor
    {
        private void OnSceneGUI()
        {
            var creatureMeasures = (CreatureMeasures) target;
            var position = creatureMeasures.transform.position;

            Handles.color = Color.white;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, creatureMeasures.Radius);
            Handles.DrawWireArc(position + new Vector3(0, creatureMeasures.Height, 0), Vector3.up, Vector3.forward, 360, creatureMeasures.Radius);

            // Debug.Log($"Draw at '{position}', with radius: '{creatureMeasures.Radius}'.");
        }
    }
}
#endif