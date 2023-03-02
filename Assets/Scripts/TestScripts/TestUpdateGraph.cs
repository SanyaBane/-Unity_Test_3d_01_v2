using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpdateGraph : MonoBehaviour
{
    private float nextActionTime = 0.0f;
    public float period = 0.5f;

    void Update()
    {
        if (Time.time > nextActionTime ) {
            nextActionTime += period;
            
            var graphToScan = AstarPath.active.data.navmesh;
            AstarPath.active.Scan(graphToScan);
                
            // Debug.Log($"_collider.bounds: {_collider.bounds}");
        }  
    }
}
