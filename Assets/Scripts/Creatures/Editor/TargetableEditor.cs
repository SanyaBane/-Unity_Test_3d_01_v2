#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [CustomEditor(typeof(Targetable))]
    public class TargetableEditor : Editor
    {
        private void OnSceneGUI()
        {
            // var targetable = (Targetable) target;
            //
            // Handles.color = Color.white;
            // Handles.DrawWireArc(targetable.transform.position, Vector3.up, Vector3.forward, 360, ep.Radius);

            // var obj = Selection.activeGameObject;
            // if (obj != null)
            // {
            //     Debug.Log(obj.gameObject.name);
            // }
        }
    }
}
#endif