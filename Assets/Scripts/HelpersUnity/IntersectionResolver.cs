using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class IntersectionResolver
    {
        // https://stackoverflow.com/a/59449849/3867255
        // public static bool LineLineIntersection(out Vector3? intersection, Vector3 linePoint1,
        //     Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        // {
        //     Vector3 lineVec3 = linePoint2 - linePoint1;
        //     Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        //     Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
        //
        //     float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
        //
        //     //is coplanar, and not parallel
        //     if (Mathf.Abs(planarFactor) < 0.0001f
        //         && crossVec1and2.sqrMagnitude > 0.0001f)
        //     {
        //         float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
        //         intersection = linePoint1 + (lineVec1 * s);
        //         return true;
        //     }
        //     else
        //     {
        //         intersection = null;
        //         return false;
        //     }
        // }
        
        public static bool LineLineIntersection(out Vector3? intersection, Vector3 line1Point1, Vector3 line1Point2,
            Vector3 line2Point1, Vector3 line2Point2)
        {
            Vector3 lineVec1 = line1Point2 - line1Point1;
            Vector3 lineVec2 = line2Point2 - line2Point1;

            Vector3 lineVec3 = line2Point2 - line1Point1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = line1Point1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = null;
                return false;
            }
        }
       
        public static bool LineLineIntersectionIgnoreY(out Vector3? intersection, Vector3 line1Point1, Vector3 line1Point2,
            Vector3 line2Point1, Vector3 line2Point2)
        {
            line1Point1 = new Vector3(line1Point1.x, 0, line1Point1.z);
            line1Point2 = new Vector3(line1Point2.x, 0, line1Point2.z);
            line2Point1 = new Vector3(line2Point1.x, 0, line2Point1.z);
            line2Point2 = new Vector3(line2Point2.x, 0, line2Point2.z);

            return LineLineIntersection(out intersection, line1Point1, line1Point2, line2Point1, line2Point2);
        }
        
        // http://answers.unity.com/answers/1658313/view.html
        public static Vector3[] IntersectionLineAndCircle(Vector3 linePoint1, Vector3 linePoint2, Vector3 circleCenter, float circleRadius)
        {
            Vector3 dp = new Vector3();
            //  get the distance between X and Z on the segment
            dp.x = linePoint2.x - linePoint1.x;
            dp.z = linePoint2.z - linePoint1.z;

            //   I don't get the math here
            float a = dp.x * dp.x + dp.z * dp.z;
            float b = 2 * (dp.x * (linePoint1.x - circleCenter.x) + dp.z * (linePoint1.z - circleCenter.z));

            float c = circleCenter.x * circleCenter.x + circleCenter.z * circleCenter.z;
            c += linePoint1.x * linePoint1.x + linePoint1.z * linePoint1.z;
            c -= 2 * (circleCenter.x * linePoint1.x + circleCenter.z * linePoint1.z);
            c -= circleRadius * circleRadius;

            var determinate = b * b - 4 * a * c; //bb4ac
            if (Mathf.Abs(a) < float.Epsilon || determinate < 0)
            {
                //  line does not intersect
                // return null;
                return new Vector3[0];
            }

            float mu1 = (-b + Mathf.Sqrt(determinate)) / (2 * a);
            float mu2 = (-b - Mathf.Sqrt(determinate)) / (2 * a);

            var ret = new Vector3[2];
            ret[0] = new Vector3(linePoint1.x + mu1 * (linePoint2.x - linePoint1.x), 0, linePoint1.z + mu1 * (linePoint2.z - linePoint1.z));
            ret[1] = new Vector3(linePoint1.x + mu2 * (linePoint2.x - linePoint1.x), 0, linePoint1.z + mu2 * (linePoint2.z - linePoint1.z));

            return ret;
        }

        public static Vector3[] IntersectionLineAndCircleWithOffset(Vector3 linePoint1, Vector3 linePoint2, Vector3 circleCenter, float circleRadius, float offset, bool inverseVector3Up)
        {
            var ret = IntersectionLineAndCircle(linePoint1, linePoint2, circleCenter, circleRadius);

            var lineDirection = (linePoint2 - linePoint1).normalized;
            
            Vector3 vector3Up = Vector3.up;
            if (inverseVector3Up)
            {
                vector3Up *= -1;
            }

            for (int i = 0; i < ret.Length; i++)
            {
                var point = ret[i];

                var perpendicularLine = Vector3.Cross(lineDirection, vector3Up).normalized;

                var offsetDirection = (perpendicularLine + lineDirection).normalized;

                point = point + offsetDirection * offset;

                ret[i] = point;
            }

            return ret;
        }

        public static Vector3[] IntersectionLineAndCircleAndOutsideOfAnotherCirclePieArea(Vector3 linePoint1, Vector3 linePoint2, Vector3 circleCenter, float circleRadius,
            Vector3 secondCircleCenter, float secondCircleRadius, Vector3 secondCircleForward, float secondCircleClockwiseRotationOffset, float secondCirclePieAngle)
        {
            Vector3[] points = IntersectionLineAndCircle(linePoint1, linePoint2, circleCenter, circleRadius);
            if (points.Length == 0)
            {
                return null;
            }

            List<Vector3> ret = new List<Vector3>();

            foreach (var point in points)
            {
                if (MathHelper.CheckPointInsideSpherePie(point, secondCircleCenter, secondCircleRadius, secondCirclePieAngle, secondCircleClockwiseRotationOffset, secondCircleForward) == MathHelper.ResultPointInsideSpherePie.InsideCircleNotPieArea)
                {
                    ret.Add(point);
                }
            }

            if (ret.Count == 0)
                return null;

            return ret.ToArray();
        }
    }
}