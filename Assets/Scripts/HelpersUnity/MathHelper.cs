using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace Assets.Scripts.HelpersUnity
{
    public static class MathHelper
    {
        // public static Vector2[] PointsTwoCirclesIntersect(Vector2 circlePos1, Vector2 circlePos2, float circleRadius1, float circleRadius2)
        // {
        //     Vector2[] intersections;
        //
        //     Intersect(circlePos1, circleRadius1, circlePos2, circleRadius2, out intersections);
        //
        //     Debug.Log(intersections);
        //
        //     return intersections;
        // }

        public enum EPointTwoCircleIntersectResult
        {
            NoIntersections = 0,
            OneIntersection = 1,
            TwoIntersections = 2
        }

        // https://gist.github.com/jupdike/bfe5eb23d1c395d8a0a1a4ddd94882ac#gistcomment-2658696
        public static EPointTwoCircleIntersectResult PointsTwoCirclesIntersect(Vector2 circleA, float radiusA,
            Vector2 circleB, float radiusB, out Vector2[] intersections)
        {
            float centerDx = circleA.x - circleB.x;
            // float centerDy = circleB.y - circleB.y;
            float centerDy = circleA.y - circleB.y;
            float r = Mathf.Sqrt(centerDx * centerDx + centerDy * centerDy);

            // no intersection
            if (!(Mathf.Abs(radiusA - radiusB) <= r && r <= radiusA + radiusB))
            {
                intersections = new Vector2[0];
                return EPointTwoCircleIntersectResult.NoIntersections;
            }

            float r2d = r * r;
            float r4d = r2d * r2d;
            float rASquared = radiusA * radiusA;
            float rBSquared = radiusB * radiusB;
            float a = (rASquared - rBSquared) / (2 * r2d);
            float r2r2 = (rASquared - rBSquared);
            float c = Mathf.Sqrt(2 * (rASquared + rBSquared) / r2d - (r2r2 * r2r2) / r4d - 1);

            float fx = (circleA.x + circleB.x) / 2 + a * (circleB.x - circleA.x);
            float gx = c * (circleB.y - circleA.y) / 2;
            float ix1 = fx + gx;
            float ix2 = fx - gx;

            float fy = (circleA.y + circleB.y) / 2 + a * (circleB.y - circleA.y);
            float gy = c * (circleA.x - circleB.x) / 2;
            float iy1 = fy + gy;
            float iy2 = fy - gy;

            // if gy == 0 and gx == 0 then the circles are tangent and there is only one solution
            if (Mathf.Abs(gx) < float.Epsilon && Mathf.Abs(gy) < float.Epsilon)
            {
                intersections = new[]
                {
                    new Vector2(ix1, iy1)
                };
                return EPointTwoCircleIntersectResult.OneIntersection;
            }

            intersections = new[]
            {
                new Vector2(ix1, iy1),
                new Vector2(ix2, iy2),
            };
            return EPointTwoCircleIntersectResult.TwoIntersections;
        }

        public static bool IsPointInsideSphere(Vector3 pointPos, Vector3 spherePos, float sphereRadius)
        {
            if (VectorHelper.DistanceSquared(pointPos, spherePos) < sphereRadius * sphereRadius)
                return true;

            return false;
        }

        public enum ResultPointInsideSpherePie
        {
            NotInsideSphere = 0,
            InsideCircleNotPieArea = 1,
            InsideCirclePieArea = 2,
        }

        public static ResultPointInsideSpherePie CheckPointInsideSpherePie(Vector3 pointPos, Vector3 spherePos, float sphereRadius, float pieAngle, float clockwiseRotationOffset, Vector3 sphereForwardVector)
        {
            var isPointInsideSphere = IsPointInsideSphere(pointPos, spherePos, sphereRadius);
            if (!isPointInsideSphere)
            {
                return ResultPointInsideSpherePie.NotInsideSphere;
            }

            var directionFromSphereToPoint = (pointPos - spherePos).normalized;
            directionFromSphereToPoint = new Vector3(directionFromSphereToPoint.x, 0, directionFromSphereToPoint.z); // reset Y axis

            var forwardVectorClockwiseRotated = Quaternion.Euler(0, clockwiseRotationOffset, 0) * sphereForwardVector;
            forwardVectorClockwiseRotated = new Vector3(forwardVectorClockwiseRotated.x, 0, forwardVectorClockwiseRotated.z); // reset Y axis

            float dotProduct = Vector3.Dot(directionFromSphereToPoint, forwardVectorClockwiseRotated);
            float dotProductDegrees = Mathf.Acos(dotProduct) * 2 * Mathf.Rad2Deg;

            if (dotProductDegrees < pieAngle)
            {
                return ResultPointInsideSpherePie.InsideCirclePieArea;
            }
            else
            {
                return ResultPointInsideSpherePie.InsideCircleNotPieArea;
            }
        }
        
        // public static ResultPointInsideSpherePie
    }
}