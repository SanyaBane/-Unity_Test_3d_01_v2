using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreateMesh : MonoBehaviour
{
    private void Start()
    {
        float width = 10;
        float height = 5;

        GameObject plane = new GameObject("Plane");
        MeshFilter meshFilter = (MeshFilter) plane.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = CreateMesh(width, height);
        
        MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
        
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.green);
        tex.Apply();
        
        renderer.material.mainTexture = tex;
        renderer.material.color = Color.green;
    }

    private Mesh CreateMesh(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        m.vertices = new Vector3[]
        {
            new Vector3(-width, 0.01f, -height),
            new Vector3(width, 0.01f, -height),
            new Vector3(width, 0.01f, height),
            new Vector3(-width, 0.01f, height)
        };
        m.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        m.triangles = new int[] {0, 1, 2, 0, 2, 3};
        m.RecalculateNormals();

        return m;
    }
}