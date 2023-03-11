using Assets.Scripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HitboxAssignerHelper))]
public class HitboxAssignerHelper_GUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (HitboxAssignerHelper)target;

        if (GUILayout.Button("Create hitboxes for matching meshes"))
        {
            script.CreateHitboxes();
        }

        if (GUILayout.Button("Remove hitboxes for matching meshes"))
        {
            script.RemoveHitboxes();
        }


        if (GUILayout.Button("Show matching meshes MeshRenderers"))
        {
            script.ShowMeshRendererForHitboxes();
        }

        if (GUILayout.Button("Hide matching meshes MeshRenderers"))
        {
            script.HideMeshRendererForHitboxes();
        }
        
        if (GUILayout.Button("Set hitboxes to \"Default\" layer."))
        {
            script.SetHitboxesToSelectableLayer(LayerManager.LAYER_INDEX_DEFAULT);
        }
        
        if (GUILayout.Button("Set hitboxes to \"Selectable\" layer."))
        {
            script.SetHitboxesToSelectableLayer(LayerManager.LAYER_INDEX_SELECTABLE);
        }
    }
}