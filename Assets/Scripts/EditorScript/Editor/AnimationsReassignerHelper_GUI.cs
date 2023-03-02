using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationsReassignerHelper))]
public class AnimationsReassignerHelper_GUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (AnimationsReassignerHelper)target;
        if (GUILayout.Button("Reassign animations"))
        {
            script.DoWork();
        }
    }
}