using UnityEngine;

namespace Assets.Scripts.AreaOfEffects
{
    public class AOECircleDebugIntersectionLineWithPieLinesNotPieArea
    {
        public Color Color;

        public Vector3 Line1Point1;
        public Vector3 Line1Point2;

        public Vector3 PieLinesStart;
        public Vector3 PieLine1Point2;
        public Vector3 PieLine2Point2;

        public Vector3? Intersection1;
        public Vector3? Intersection1WithOffset;
        public Vector3? Intersection2;
        public Vector3? Intersection2WithOffset;

        public float Radius;

        // public bool DisplayTestInfo = false;
        //
        // public Vector3 test1PointBegin;
        // public Vector3 test1Destination1;
        // public Vector3 test1Destination2;
        // public Vector3 test1Destination3;
        //
        // public Vector3 test2PointBegin;
        // public Vector3 test2Destination1;
        // public Vector3 test2Destination2;
        // public Vector3 test2Destination3;
        //
        // public Vector3 test1Line1Start;
        // public Vector3 test1Line1Direction;
        // public Vector3 test1Line2Start;
        // public Vector3 test1Line2Direction;
        //
        // public Vector3 test2Line1Start;
        // public Vector3 test2Line1Direction;
        // public Vector3 test2Line2Start;
        // public Vector3 test2Line2Direction;

        public AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color color)
        {
            this.Color = color;
        }

        public void DisplayGizmos()
        {
            Gizmos.color = this.Color;

            float mult = 3;

            Gizmos.DrawRay(this.PieLinesStart, (this.PieLine1Point2 - this.PieLinesStart).normalized * this.Radius * mult);
            Gizmos.DrawRay(this.PieLinesStart, (this.PieLine2Point2 - this.PieLinesStart).normalized * this.Radius * mult);

            Gizmos.DrawRay(this.PieLinesStart, (this.PieLinesStart - this.PieLine1Point2).normalized * this.Radius * mult);
            Gizmos.DrawRay(this.PieLinesStart, (this.PieLinesStart - this.PieLine2Point2).normalized * this.Radius * mult);

            Gizmos.DrawLine(this.Line1Point1, this.Line1Point2);

            if (this.Intersection1 != null)
            {
                Gizmos.DrawSphere(this.Intersection1.Value, 0.1f);
                Gizmos.DrawSphere(this.Intersection1WithOffset.Value, 0.05f);
            }

            if (this.Intersection2 != null)
            {
                Gizmos.DrawSphere(this.Intersection2.Value, 0.1f);
                Gizmos.DrawSphere(this.Intersection2WithOffset.Value, 0.05f);
            }
        }

        public void Reset()
        {
            // test1PointBegin = Vector3.zero;
            // test1Destination1 = Vector3.zero;
            // test1Destination2 = Vector3.zero;
            // test1Destination3 = Vector3.zero;
            //
            // test2PointBegin = Vector3.zero;
            // test2Destination1 = Vector3.zero;
            // test2Destination2 = Vector3.zero;
            // test2Destination3 = Vector3.zero;
            //
            // test1Line1Start = Vector3.zero;
            // test1Line1Direction = Vector3.zero;
            // test1Line2Start = Vector3.zero;
            // test1Line2Direction = Vector3.zero;
            //
            // test2Line1Start = Vector3.zero;
            // test2Line1Direction = Vector3.zero;
            // test2Line2Start = Vector3.zero;
            // test2Line2Direction = Vector3.zero;
        }
    }
}