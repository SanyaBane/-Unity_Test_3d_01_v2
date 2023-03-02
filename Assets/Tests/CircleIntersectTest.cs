using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HelpersUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CircleIntersectTest
    {
        [Test]
        public void TestPointsTwoCirclesIntersect()
        {
            Vector2[] retIntersectionVectors;
            var ret = MathHelper.PointsTwoCirclesIntersect(
                new Vector2(3, 3), 3,
                new Vector2(5, 8), 4,
                out retIntersectionVectors);

            var expected = new Vector2[] {new Vector2(9.5f, -1.8f), new Vector2(-5.0f, 4.0f)};

            Assert.AreEqual(ret, MathHelper.EPointTwoCircleIntersectResult.TwoIntersections);
            Assert.AreEqual(retIntersectionVectors[0].x, expected[0].x, 0.05f);
            Assert.AreEqual(retIntersectionVectors[0].y, expected[0].y, 0.05f);
            Assert.AreEqual(retIntersectionVectors[1].x, expected[1].x, 0.05f);
            Assert.AreEqual(retIntersectionVectors[1].y, expected[1].y, 0.05f);
        }
        
        [Test]
        public void TestIsPointInsideSphere()
        {
            Vector3 point;
            Vector3 sphereLocation;
            float sphereRadius;
            bool isPointInsideSphere;
            
            point = new Vector3(1, 0, 0);
            sphereLocation = new Vector3(2, 0, 0);
            sphereRadius = 1;
            isPointInsideSphere = MathHelper.IsPointInsideSphere(point, sphereLocation, sphereRadius);
            Assert.IsFalse(isPointInsideSphere);
            
            point = new Vector3(1, 0, 0);
            sphereLocation = new Vector3(2, 0, 0);
            sphereRadius = 1.1f;
            isPointInsideSphere = MathHelper.IsPointInsideSphere(point, sphereLocation, sphereRadius);
            Assert.IsTrue(isPointInsideSphere);
        }

        [Test]
        public void TestIntersectionLineAndCircleOutsidePieArea()
        {
            Vector3 lineStart1 = new Vector3(0, 0, 0.85f);
            Vector3 lineEnd1 = new Vector3(-7.77f, 0, -6.92f);
            Vector3 circleCenter = new Vector3(0, 0, 3.93f);
            float circleRadius = 2.5f;
            Vector3 circleForward = new Vector3(0, 0, 1);
            float cirlceClockwiseRotation = 0;
            float circlePieAngle = 270;
            //
            // VectorHelper.IntersectionLineAndCircleOutsidePieArea()
        }
    }
}