using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public List<ParticleSystem> ParticlesRadius = new List<ParticleSystem>();

    // public float Radius;

    // public void SetRadius(float radius)
    // {
    //     foreach (var particle in ParticlesRadius)
    //     {
    //         var shape = particle.shape;
    //
    //         if (!shape.enabled)
    //             continue;
    //
    //         shape.radius = radius;
    //     }
    // }
}