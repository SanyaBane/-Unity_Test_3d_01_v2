using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class VectorHelper
    {
        /// <summary>
        /// Use only to compare with another result of this method.
        /// </summary>
        public static float DistanceForComparison(Vector3 a, Vector3 b)
        {
#if DEBUG
            float ret = Vector3.Distance(a, b);
            return ret;
#else
            float ret = DistanceSquared(a, b);
            return ret;
#endif
        }

        public static float GetDistanceComparisonValue(float value)
        {
#if DEBUG
            return value;
#else
            return value * value;
#endif
        }
        
        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            float ret = (a - b).sqrMagnitude;
            return ret;
        }

        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            float ret = (a - b).sqrMagnitude;
            return ret;
        }

        public static Vector3 GetMeanVector(Vector3[] positions)
        {
            if (positions.Length == 0)
                return Vector3.zero;

            float x = 0f;
            float y = 0f;
            float z = 0f;

            foreach (Vector3 pos in positions)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
            }

            var ret = new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
            return ret;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point;
        }

        public static Vector3 Vector2ToVector3XZ(Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }

        public static Vector3 DirFromAngle(float angleInDegrees, Vector3 eulerAngles, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        
    }
}