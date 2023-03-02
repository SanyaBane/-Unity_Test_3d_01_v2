﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorPlayerHelper))]
public class EditorPlayerHelper_GUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (EditorPlayerHelper)target;
        if (GUILayout.Button("Select Gameobject by Name"))
        {
            script.EditorSelect();
        }
    }
}