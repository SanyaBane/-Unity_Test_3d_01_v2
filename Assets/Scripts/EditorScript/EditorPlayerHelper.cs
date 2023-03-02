using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorPlayerHelper : MonoBehaviour
{
    public string SelectChildName = "WeaponSlot";

    public void EditorSelect()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == SelectChildName)
            {
#if UNITY_EDITOR
                Selection.activeGameObject = child.gameObject;
#endif
            }
        }
    }
}