using System.Collections.Generic;
using UnityEngine;

public class HitboxAssignerHelper : MonoBehaviour
{
    public string HitboxPrefix = "m_hitbox_";


    public void CreateHitboxes()
    {
        var matchedGameObjects = GetGameObjectsWithMatchingName(HitboxPrefix);

        RemoveHitboxesForGameObjects(matchedGameObjects);
        CreateHitboxesForGameObjects(matchedGameObjects);
    }

    // Friendly reminder - method do not create new gameobjects,
    // it just adds "BoxCollider"-component to gameobjects which are match user's input name
    private void CreateHitboxesForGameObjects(List<Transform> children)
    {
        foreach (var child in children)
        {
            // if any BoxColliders already exist for this mesh, destroy them
            var boxCollidersArray = child.GetComponents<BoxCollider>();
            for (int i = 0; i < boxCollidersArray.Length; i++)
            {
                DestroyImmediate(boxCollidersArray[i]);
            }

            // add BoxCollider which will work as hitbox
            var boxCollider = child.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
    }

    public void RemoveHitboxes()
    {
        var children = GetGameObjectsWithMatchingName(HitboxPrefix);
        RemoveHitboxesForGameObjects(children);
    }

    public void RemoveHitboxesForGameObjects(List<Transform> children)
    {
        foreach (var child in children)
        {
            // if any BoxColliders already exist for this mesh, destroy them
            var boxCollidersArray = child.GetComponents<BoxCollider>();
            for (int i = 0; i < boxCollidersArray.Length; i++)
            {
                DestroyImmediate(boxCollidersArray[i]);
            }
        }
    }

    public void ShowMeshRendererForHitboxes()
    {
        ShowOrHideMeshRendererForHitboxes(true);
    }

    public void HideMeshRendererForHitboxes()
    {
        ShowOrHideMeshRendererForHitboxes(false);
    }

    private void ShowOrHideMeshRendererForHitboxes(bool isShow)
    {
        var children = GetGameObjectsWithMatchingName(HitboxPrefix);
        foreach (var child in children)
        {
            var meshRenderer = child.GetComponent<MeshRenderer>();
            meshRenderer.enabled = isShow;
        }
    }

    private List<Transform> GetGameObjectsWithMatchingName(string name)
    {
        var ret = new List<Transform>();

        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name.IndexOf(name) == 0) // if found mesh with name pattern which matches needed
            {
                ret.Add(child);
            }
        }

        return ret;
    }

    public void SetHitboxesToSelectableLayer(int layerIndex)
    {
        var matchedGameObjects = GetGameObjectsWithMatchingName(HitboxPrefix);
        SetHitboxesToSelectableLayer(matchedGameObjects, layerIndex);
    }
    
    private void SetHitboxesToSelectableLayer(List<Transform> children, int layerIndex)
    {
        foreach (var child in children)
        {
            child.gameObject.layer = layerIndex;
        }
    }
}