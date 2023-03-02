using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HelpersUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    [CustomEditor(typeof(EllipseProjection))]
    public class EllipseProjectionEditor : Editor
    {
        private void OnSceneGUI()
        {
            var ep = (EllipseProjection) target;
            var transform = ep.transform;
            var position = transform.position;
            var eulerAngles = transform.eulerAngles;

            Handles.color = Color.white;

            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, ep.Radius);

            Vector3 viewAngleA = VectorHelper.DirFromAngle(-ep.Angle / 2 + ep.ClockwiseRotation, eulerAngles, false);
            Vector3 viewAngleB = VectorHelper.DirFromAngle(ep.Angle / 2 + ep.ClockwiseRotation, eulerAngles, false);

            Vector3 outsidePointAtAngleA = position + viewAngleA * ep.Radius;
            Vector3 outsidePointAtAngleB = position + viewAngleB * ep.Radius;

            Handles.DrawLine(position, outsidePointAtAngleA);
            Handles.DrawLine(position, outsidePointAtAngleB);
        }
    }
}