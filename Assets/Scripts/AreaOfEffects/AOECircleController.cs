using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Managers;
using Assets.Scripts.NPC;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.AreaOfEffects
{
    public class AOECircleController : MonoBehaviour, IAreaOfEffect
    {
        // в чём смысл нескольких тегов - для некоторых аое, в зависимости от позиции ноды относительно аое, должен быть разный штраф (тег)
        // В случае "AOECircleController" - самые центральные штрафные ноды должны быть наименее штрафные. К примеру:
        // 1. Ближайшие - 28000
        // 2. На 1 ширину дальше - 29000
        // 2. На 2 ширины и более - 30000
        public const uint PARTY_MEMBER_NODE_TAG_ENEMY_AOE_1 = 2;
        public const uint PARTY_MEMBER_NODE_TAG_ENEMY_AOE_2 = 3;
        public const uint PARTY_MEMBER_NODE_TAG_ENEMY_AOE_3 = 4;

        private EllipseProjection _ellipseProjection;
        public float Radius { get; private set; }
        public Vector3 Forward { get; private set; }
        public Vector3 Center { get; private set; }
        public float Angle { get; private set; }
        public float ClockwiseRotation { get; private set; }

        private AOECircleGraphUpdateObject _graphUpdateObject;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            // Gizmos.color = Color.green;
            // var pieOuterPoints = this.GetPieOuterPoints();
            // Gizmos.DrawSphere(pieOuterPoints[0], 0.1f);
            // Gizmos.DrawSphere(pieOuterPoints[1], 0.1f);
        }

        public Vector3[] GetPieOuterPoints()
        {
            var transform1 = this.transform;
            var aoeCircleControllerEulerAngles = transform1.eulerAngles;
            var aoeCircleControllerPosition = transform1.position;

            Vector3 dir1 = VectorHelper.DirFromAngle(-this.Angle / 2 + this.ClockwiseRotation, aoeCircleControllerEulerAngles, false);
            Vector3 dir2 = VectorHelper.DirFromAngle(this.Angle / 2 + this.ClockwiseRotation, aoeCircleControllerEulerAngles, false);

            Vector3 point1 = aoeCircleControllerPosition + dir1 * Radius;
            Vector3 point2 = aoeCircleControllerPosition + dir2 * Radius;

            return new Vector3[] {point1, point2};
        }

        public Vector3 GetSafeSpotAtCircleCenter(float offsetFromAOE)
        {
            var forwardVectorClockwiseRotated = Quaternion.Euler(0, this.ClockwiseRotation, 0) * this.Forward;
            forwardVectorClockwiseRotated = new Vector3(forwardVectorClockwiseRotated.x, 0, forwardVectorClockwiseRotated.z); // reset Y axis

            var ret = this.Center + (-forwardVectorClockwiseRotated * offsetFromAOE);
            return ret;
        }

        public Vector3 GetSafeSpotOutsideAOE(Vector3 direction, float offsetFromAOE)
        {
            Vector3 ret = this.Center + direction * (this.Radius + offsetFromAOE);
            return ret;
        }


        public Vector3[] IntersectionLineWithPieLinesNotPieArea(Vector3 linePoint1Pos, Vector3 linePoint2Pos, float offsetFromAOE,
            AOECircleDebugIntersectionLineWithPieLinesNotPieArea debugIntersectionLineWithPieLinesNotPieArea, bool useOffsetBetweenLines, bool offsetInversePieLineDirection, bool inverseVector3Up)
        {
            var ret = new List<Vector3>();

            var lineDirection = (linePoint2Pos - linePoint1Pos).normalized;

            var pieOuterPoints = GetPieOuterPoints();

            if (debugIntersectionLineWithPieLinesNotPieArea != null)
            {
                debugIntersectionLineWithPieLinesNotPieArea.Reset();

                debugIntersectionLineWithPieLinesNotPieArea.Radius = this.Radius;

                debugIntersectionLineWithPieLinesNotPieArea.Line1Point1 = linePoint1Pos;
                debugIntersectionLineWithPieLinesNotPieArea.Line1Point2 = linePoint2Pos;

                debugIntersectionLineWithPieLinesNotPieArea.PieLinesStart = this.Center;
                debugIntersectionLineWithPieLinesNotPieArea.PieLine1Point2 = pieOuterPoints[0];
                debugIntersectionLineWithPieLinesNotPieArea.PieLine2Point2 = pieOuterPoints[1];
            }

            Vector3? intersection1;
            IntersectionResolver.LineLineIntersectionIgnoreY(out intersection1, linePoint1Pos, linePoint2Pos, this.Center, pieOuterPoints[0]);
            Vector3? intersection2;
            IntersectionResolver.LineLineIntersectionIgnoreY(out intersection2, linePoint1Pos, linePoint2Pos, this.Center, pieOuterPoints[1]);

            var vector3Up = Vector3.up;
            if (inverseVector3Up)
            {
                vector3Up *= -1;
            }

            if (intersection1 != null)
            {
                var pieLineDirection = (pieOuterPoints[0] - this.Center).normalized;
                if (offsetInversePieLineDirection)
                {
                    pieLineDirection *= -1;
                }

                Vector3 offsetDirection;
                if (useOffsetBetweenLines)
                {
                    offsetDirection = (pieLineDirection + lineDirection).normalized * -1;
                }
                else
                {
                    offsetDirection = Vector3.Cross(pieLineDirection, -vector3Up).normalized;
                }

                if (debugIntersectionLineWithPieLinesNotPieArea != null)
                {
                    debugIntersectionLineWithPieLinesNotPieArea.Intersection1 = intersection1.Value;
                }

                intersection1 = intersection1.Value + offsetDirection * offsetFromAOE;

                if (debugIntersectionLineWithPieLinesNotPieArea != null)
                {
                    debugIntersectionLineWithPieLinesNotPieArea.Intersection1WithOffset = intersection1.Value;
                }

                if (MathHelper.CheckPointInsideSpherePie(intersection1.Value, this.Center, this.Radius, this.Angle, this.ClockwiseRotation, this.Forward) ==
                    MathHelper.ResultPointInsideSpherePie.InsideCircleNotPieArea)
                {
                    ret.Add(intersection1.Value);
                }
            }

            if (intersection2 != null)
            {
                var pieLineDirection = (pieOuterPoints[1] - this.Center).normalized;
                if (offsetInversePieLineDirection)
                {
                    pieLineDirection *= -1;
                }

                Vector3 offsetDirection;
                if (useOffsetBetweenLines)
                {
                    offsetDirection = (pieLineDirection * -1 + lineDirection).normalized * -1;
                }
                else
                {
                    offsetDirection = Vector3.Cross(pieLineDirection, vector3Up).normalized;
                }

                if (debugIntersectionLineWithPieLinesNotPieArea != null)
                {
                    debugIntersectionLineWithPieLinesNotPieArea.Intersection2 = intersection2.Value;
                }

                intersection2 = intersection2.Value + offsetDirection * offsetFromAOE;

                if (debugIntersectionLineWithPieLinesNotPieArea != null)
                {
                    debugIntersectionLineWithPieLinesNotPieArea.Intersection2WithOffset = intersection2.Value;
                }

                if (MathHelper.CheckPointInsideSpherePie(intersection2.Value, this.Center, this.Radius, this.Angle, this.ClockwiseRotation, this.Forward) ==
                    MathHelper.ResultPointInsideSpherePie.InsideCircleNotPieArea)
                {
                    ret.Add(intersection2.Value);
                }
            }

            return ret.ToArray();
        }

        private void Start()
        {
            _ellipseProjection = this.GetComponent<EllipseProjection>();

            GameManager.Instance.AOEManager.AddNewElementToAORCircleControllers(this);
        }

        private void Update()
        {
            Radius = _ellipseProjection.Radius;
            Angle = _ellipseProjection.Angle;
            ClockwiseRotation = _ellipseProjection.ClockwiseRotation;

            Center = this.transform.position;
            Forward = this.transform.forward;
        }

        public void UpdateTagsInfo()
        {
            ResetTags();

            var bounds = new Bounds(Center, new Vector3(Radius * 2, Radius * 2, Radius * 2));
            _graphUpdateObject = new AOECircleGraphUpdateObject(this, this.Forward, bounds, _ellipseProjection.Radius);

            AstarPath.active.UpdateGraphs(_graphUpdateObject);
        }

        private void ResetTags()
        {
            if (_graphUpdateObject != null)
            {
                try
                {
                    if (_graphUpdateObject.changedNodes != null)
                    {
                        foreach (var node in _graphUpdateObject.changedNodes)
                        {
                            node.Tag = 0; //reset tags
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        private void OnDestroy()
        {
            ResetTags();
        }
        
        public static Vector3[] GetSafePointsForSingleAOE(NpcAI npcAI, AOECircleController aoeCircleController, Vector3 creaturePosition, Vector3 targetPosition, float offsetFromAOE)
        {
            var ret = new List<Vector3>();

            var aoePieOuterPoints = aoeCircleController.GetPieOuterPoints();

            // 1. Get nearest point at ouside of circle in direciton to "target".
            var pointsOutsideAOECircles = new List<Vector3>();
            Vector3 directionFromAOECenterToTarget = (targetPosition - aoeCircleController.Center).normalized;
            pointsOutsideAOECircles.Add(aoeCircleController.GetSafeSpotOutsideAOE(directionFromAOECenterToTarget, offsetFromAOE));

            ret.AddRange(pointsOutsideAOECircles);
            npcAI.ListSafePointsOutsideAOECircles.AddRange(pointsOutsideAOECircles);

            // 2. If CirclePie Angle is lesser then '360', then do previous options, plus:
            //  2a. Vector from "creature" to "target" - intersection with both (2) "pie" lines - but only if point is inside "SafetyPieArea" (inside circle radius, but outside PieArea).
            //  2b. Perpendicular vector from "target" to both (2) "pie" lines - but only if point is inside "SafetyPieArea" (inside circle radius, but outside PieArea).

            // 2a.
            var test2a = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.magenta);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2a);

            Vector3[] intersectionsCreatureTargetLineAndAOEPieLines = aoeCircleController.IntersectionLineWithPieLinesNotPieArea(creaturePosition, targetPosition, offsetFromAOE,
                test2a, false, false, false);
            ret.AddRange(intersectionsCreatureTargetLineAndAOEPieLines);

            // 2b.
            var test2b1 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            var test2b2 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2b1);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2b2);

            var perpendicularLineFromTargetToPieLine1 = targetPosition + Vector3.Cross((aoeCircleController.Center - aoePieOuterPoints[0]).normalized, Vector3.up);
            var perpendicularLineFromTargetToPieLine2 = targetPosition + Vector3.Cross((aoeCircleController.Center - aoePieOuterPoints[1]).normalized, Vector3.up);

            Vector3[] intersectionsPerpendicularTargetPie1AndAOEPieLines = aoeCircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine1, offsetFromAOE,
                test2b1, false, false, true);
            ret.AddRange(intersectionsPerpendicularTargetPie1AndAOEPieLines);

            Vector3[] intersectionsPerpendicularTargetPie2AndAOEPieLines = aoeCircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine2, offsetFromAOE,
                test2b2, false, false, true);
            ret.AddRange(intersectionsPerpendicularTargetPie2AndAOEPieLines);

            // 3. Use it's center point (with offset) as possible safe point.
            var centerOfAOEWithOffset = aoeCircleController.GetSafeSpotAtCircleCenter(offsetFromAOE);
            npcAI.ListAOECircleCenterWithOffset.Add(centerOfAOEWithOffset);
            ret.Add(centerOfAOEWithOffset);

            return ret.ToArray();
        }

        public static Vector3[] GetSafePointsBetweenTwoCircles(NpcAI npcAI, AOECircleController firstAOECircleController, AOECircleController secondAOECircleController,
            Vector3 creaturePosition, Vector3 targetPosition, float offsetFromAOE, float radiusOffset)
        {
            // If CirclePie Angle is '360':

            var firstCirclePositions = new Vector2(firstAOECircleController.Center.x, firstAOECircleController.Center.z);
            var secondCirclePositions = new Vector2(secondAOECircleController.Center.x, secondAOECircleController.Center.z);

            var firstAOEPieOuterPoints = firstAOECircleController.GetPieOuterPoints();
            var secondAOEPieOuterPoints = secondAOECircleController.GetPieOuterPoints();

            var localPossibleSafePoints = new List<Vector3>();

            // 1, 2, 5
            localPossibleSafePoints.AddRange(GetSafePointsForSingleAOE(npcAI, firstAOECircleController, creaturePosition, targetPosition, offsetFromAOE));
            localPossibleSafePoints.AddRange(GetSafePointsForSingleAOE(npcAI, secondAOECircleController, creaturePosition, targetPosition, offsetFromAOE));

            // For multiple AOE-circles, do everything above, plus:       
            // 3. Get intersection of first aoe's outside circle radius with second aoe's circle radius.
            // 4. Get intersection of first aoe's both (2) "pie" lines with second aoe's:
            //  4a. Second aoe's curle radius.
            //  4b. Second aoe's both (2) "pie" lines.

            // 2a. First AOE
            var test2a = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.magenta);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2a);

            Vector3[] intersectionsCreatureTargetLineAndFirstAOEPieLines = firstAOECircleController.IntersectionLineWithPieLinesNotPieArea(creaturePosition, targetPosition, offsetFromAOE,
                test2a, false, false, false);
            localPossibleSafePoints.AddRange(intersectionsCreatureTargetLineAndFirstAOEPieLines);

            // 2a. Second AOE
            var test2b = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.magenta);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2b);

            intersectionsCreatureTargetLineAndFirstAOEPieLines = secondAOECircleController.IntersectionLineWithPieLinesNotPieArea(creaturePosition, targetPosition, offsetFromAOE,
                test2b, false, false, false);
            localPossibleSafePoints.AddRange(intersectionsCreatureTargetLineAndFirstAOEPieLines);

            // 2b. First AOE
            var test2b1 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            var test2b2 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2b1);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test2b2);

            var perpendicularLineFromTargetToPieLine1 = targetPosition + Vector3.Cross((firstAOECircleController.Center - firstAOEPieOuterPoints[0]).normalized, Vector3.up);
            var perpendicularLineFromTargetToPieLine2 = targetPosition + Vector3.Cross((firstAOECircleController.Center - firstAOEPieOuterPoints[1]).normalized, Vector3.up);

            Vector3[] intersectionsPerpendicularTargetPie1AndFirstAOEPieLines = firstAOECircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine1, offsetFromAOE,
                test2b1, false, false, true);
            localPossibleSafePoints.AddRange(intersectionsPerpendicularTargetPie1AndFirstAOEPieLines);

            Vector3[] intersectionsPerpendicularTargetPie2AndFirstAOEPieLines = firstAOECircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine2, offsetFromAOE,
                test2b2, false, false, true);
            localPossibleSafePoints.AddRange(intersectionsPerpendicularTargetPie2AndFirstAOEPieLines);

            // 2b. Second AOE
            var test3b3 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            var test3b4 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.cyan);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test3b3);
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Add(test3b4);

            perpendicularLineFromTargetToPieLine1 = targetPosition + Vector3.Cross((secondAOECircleController.Center - secondAOEPieOuterPoints[0]).normalized, Vector3.up);
            perpendicularLineFromTargetToPieLine2 = targetPosition + Vector3.Cross((secondAOECircleController.Center - secondAOEPieOuterPoints[1]).normalized, Vector3.up);

            intersectionsPerpendicularTargetPie1AndFirstAOEPieLines = secondAOECircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine1, offsetFromAOE,
                test3b3, false, false, true);
            localPossibleSafePoints.AddRange(intersectionsPerpendicularTargetPie1AndFirstAOEPieLines);

            intersectionsPerpendicularTargetPie2AndFirstAOEPieLines = secondAOECircleController.IntersectionLineWithPieLinesNotPieArea(targetPosition, perpendicularLineFromTargetToPieLine2, offsetFromAOE,
                test3b4, false, false, true);
            localPossibleSafePoints.AddRange(intersectionsPerpendicularTargetPie2AndFirstAOEPieLines);

            // 3.
            var pointsOutsideAOECirclesIntersections = new List<Vector3>();

            Vector2[] retIntersectionPointsWithOffset;
            var retPointsTwoCirclesIntersect = MathHelper.PointsTwoCirclesIntersect(
                firstCirclePositions, firstAOECircleController.Radius + offsetFromAOE,
                secondCirclePositions, secondAOECircleController.Radius + offsetFromAOE,
                out retIntersectionPointsWithOffset);

            switch (retPointsTwoCirclesIntersect)
            {
                case MathHelper.EPointTwoCircleIntersectResult.NoIntersections:
                    // var distanceSquaredToFirst = VectorHelper.DistanceSquared(creaturePosition, firstAOECircleController.Center);
                    // var distanceSquaredToSecond = VectorHelper.DistanceSquared(creaturePosition, secondAOECircleController.Center);
                    //
                    // var nearestAOE = distanceSquaredToFirst < distanceSquaredToSecond ? firstAOECircleController : secondAOECircleController;
                    // possibleSafePoint.Add(GerNearestSafePointFromCircleAOE(creaturePosition, nearestAOE, targetPosition, offsetFromAOE, radiusOffset));
                    break;
                case MathHelper.EPointTwoCircleIntersectResult.OneIntersection:
                    pointsOutsideAOECirclesIntersections.Add(VectorHelper.Vector2ToVector3XZ(retIntersectionPointsWithOffset[0]));
                    break;
                case MathHelper.EPointTwoCircleIntersectResult.TwoIntersections:
                    pointsOutsideAOECirclesIntersections.Add(VectorHelper.Vector2ToVector3XZ(retIntersectionPointsWithOffset[0]));
                    pointsOutsideAOECirclesIntersections.Add(VectorHelper.Vector2ToVector3XZ(retIntersectionPointsWithOffset[1]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            localPossibleSafePoints.AddRange(pointsOutsideAOECirclesIntersections);
            npcAI.ListMultipleAOECirclesIntersections.AddRange(pointsOutsideAOECirclesIntersections);


            // 4a. First AOE
            var firstAOEPieLine1AndSecondAOECircleRadius = IntersectionResolver.IntersectionLineAndCircleWithOffset(firstAOECircleController.Center, firstAOEPieOuterPoints[0],
                secondAOECircleController.Center, secondAOECircleController.Radius, offsetFromAOE, false);

            var firstAOEPieLine2AndSecondAOECircleRadius = IntersectionResolver.IntersectionLineAndCircleWithOffset(firstAOECircleController.Center, firstAOEPieOuterPoints[1],
                secondAOECircleController.Center, secondAOECircleController.Radius, offsetFromAOE, true);

            localPossibleSafePoints.AddRange(firstAOEPieLine1AndSecondAOECircleRadius);
            localPossibleSafePoints.AddRange(firstAOEPieLine2AndSecondAOECircleRadius);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCircleRadius.AddRange(firstAOEPieLine1AndSecondAOECircleRadius);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCircleRadius.AddRange(firstAOEPieLine2AndSecondAOECircleRadius);

            // 4a. Second AOE
            var secondAOEPieLine1AndSecondAOECircleRadius = IntersectionResolver.IntersectionLineAndCircleWithOffset(secondAOECircleController.Center, secondAOEPieOuterPoints[0],
                firstAOECircleController.Center, firstAOECircleController.Radius, offsetFromAOE, false);

            var secondAOEPieLine2AndSecondAOECircleRadius = IntersectionResolver.IntersectionLineAndCircleWithOffset(secondAOECircleController.Center, secondAOEPieOuterPoints[1],
                firstAOECircleController.Center, firstAOECircleController.Radius, offsetFromAOE, true);

            localPossibleSafePoints.AddRange(secondAOEPieLine1AndSecondAOECircleRadius);
            localPossibleSafePoints.AddRange(secondAOEPieLine2AndSecondAOECircleRadius);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCircleRadius.AddRange(secondAOEPieLine1AndSecondAOECircleRadius);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCircleRadius.AddRange(secondAOEPieLine2AndSecondAOECircleRadius);

            // 4b. First AOE
            var test4b1 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.magenta);
            var test4b2 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.green);
            npcAI.ListDebugIntersectionAOEPieLineWithSecondAOEPieLine.Add(test4b1);
            npcAI.ListDebugIntersectionAOEPieLineWithSecondAOEPieLine.Add(test4b2);

            Vector3[] firstAOEPieLine1IntersectionsWithSecondAOEPieLines = secondAOECircleController.IntersectionLineWithPieLinesNotPieArea(firstAOECircleController.Center, firstAOEPieOuterPoints[0], offsetFromAOE,
                test4b1, true, true, false);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCirclePieLines.AddRange(firstAOEPieLine1IntersectionsWithSecondAOEPieLines);
            localPossibleSafePoints.AddRange(firstAOEPieLine1IntersectionsWithSecondAOEPieLines);

            Vector3[] firstAOEPieLine2IntersectionsWithSecondAOEPieLines = secondAOECircleController.IntersectionLineWithPieLinesNotPieArea(firstAOECircleController.Center, firstAOEPieOuterPoints[1], offsetFromAOE,
                test4b2, true, false, false);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCirclePieLines.AddRange(firstAOEPieLine2IntersectionsWithSecondAOEPieLines);
            localPossibleSafePoints.AddRange(firstAOEPieLine2IntersectionsWithSecondAOEPieLines);

            // 4b. Second AOE
            var test4b3 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.magenta);
            var test4b4 = new AOECircleDebugIntersectionLineWithPieLinesNotPieArea(Color.green);
            npcAI.ListDebugIntersectionAOEPieLineWithSecondAOEPieLine.Add(test4b3);
            npcAI.ListDebugIntersectionAOEPieLineWithSecondAOEPieLine.Add(test4b4);

            Vector3[] secondAOEPieLine1IntersectionsWithSecondAOEPieLines = firstAOECircleController.IntersectionLineWithPieLinesNotPieArea(secondAOECircleController.Center, secondAOEPieOuterPoints[0], offsetFromAOE,
                test4b3, true, true, false);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCirclePieLines.AddRange(secondAOEPieLine1IntersectionsWithSecondAOEPieLines);
            localPossibleSafePoints.AddRange(secondAOEPieLine1IntersectionsWithSecondAOEPieLines);

            Vector3[] secondAOEPieLine2IntersectionsWithSecondAOEPieLines = firstAOECircleController.IntersectionLineWithPieLinesNotPieArea(secondAOECircleController.Center, secondAOEPieOuterPoints[1], offsetFromAOE,
                test4b4, true, false, false);
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCirclePieLines.AddRange(secondAOEPieLine2IntersectionsWithSecondAOEPieLines);
            localPossibleSafePoints.AddRange(secondAOEPieLine2IntersectionsWithSecondAOEPieLines);

            return localPossibleSafePoints.ToArray();
        }
        
        public static Vector3? GerNearestSafePointFromCirclePieAOE(Vector3 creaturePosition, AOECircleController aoeCircleController, Vector3 targetPosition, float offsetFromAOE, float radiusOffset)
        {
            if (aoeCircleController.Angle >= 360)
                return null;

            var pieOuterPoints = aoeCircleController.GetPieOuterPoints();

            Vector3? intersection1;
            Vector3? intersection2;
            // TODO вместо поиска точки на прямой от игрока к мобу, лучше пустить перпендикулярную прямую на обе стороны PIE, выбрать ближайшую, и применить AOE оффсет 
            bool lineLineIntersection1 = IntersectionResolver.LineLineIntersectionIgnoreY(out intersection1, creaturePosition, targetPosition, aoeCircleController.Center, pieOuterPoints[0]);
            bool lineLineIntersection2 = IntersectionResolver.LineLineIntersectionIgnoreY(out intersection2, creaturePosition, targetPosition, aoeCircleController.Center, pieOuterPoints[1]);

            if (!lineLineIntersection1 && !lineLineIntersection2)
            {
                Vector3 ret = targetPosition + (-aoeCircleController.transform.forward * offsetFromAOE);

                return ret;
            }

            Vector3 intersection1Offset;
            Vector3 intersection2Offset;

            if (!lineLineIntersection1)
            {
                intersection1 = targetPosition;
                intersection1Offset = (-aoeCircleController.transform.forward * offsetFromAOE);
            }
            else
            {
                intersection1Offset = Vector3.Cross(pieOuterPoints[0] - aoeCircleController.Center, Vector3.up).normalized * offsetFromAOE;
            }

            if (!lineLineIntersection2)
            {
                intersection2 = targetPosition;
                intersection2Offset = (-aoeCircleController.transform.forward * offsetFromAOE);
            }
            else
            {
                intersection2Offset = Vector3.Cross(pieOuterPoints[1] - aoeCircleController.Center, -Vector3.up).normalized * offsetFromAOE;
            }

            var intersection1CheckInsidePie = MathHelper.CheckPointInsideSpherePie(intersection1.Value, aoeCircleController.Center, aoeCircleController.Radius,
                aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward);

            var intersection2CheckInsidePie = MathHelper.CheckPointInsideSpherePie(intersection2.Value, aoeCircleController.Center, aoeCircleController.Radius,
                aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward);

            var possibleSafePointInsidePieAngle = new List<Tuple<Vector3, Vector3>>();

            if (intersection1CheckInsidePie != MathHelper.ResultPointInsideSpherePie.NotInsideSphere)
            {
                var intersection1WithOffset = intersection1.Value + intersection1Offset;

                var intersection1WithOffsetCheckInsidePie = MathHelper.CheckPointInsideSpherePie(intersection1WithOffset, aoeCircleController.Center, aoeCircleController.Radius,
                    aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward);

                if (intersection1WithOffsetCheckInsidePie == MathHelper.ResultPointInsideSpherePie.InsideCircleNotPieArea)
                {
                    possibleSafePointInsidePieAngle.Add(new Tuple<Vector3, Vector3>(intersection1.Value, intersection1Offset));
                }
            }

            if (intersection2CheckInsidePie != MathHelper.ResultPointInsideSpherePie.NotInsideSphere)
            {
                var intersection2WithOffset = intersection2.Value + intersection2Offset;

                var intersection2WithOffsetCheckInsidePie = MathHelper.CheckPointInsideSpherePie(intersection2WithOffset, aoeCircleController.Center, aoeCircleController.Radius,
                    aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward);

                if (intersection2WithOffsetCheckInsidePie == MathHelper.ResultPointInsideSpherePie.InsideCircleNotPieArea)
                {
                    possibleSafePointInsidePieAngle.Add(new Tuple<Vector3, Vector3>(intersection2.Value, intersection2Offset));
                }
            }

            Vector3? closestPointOutsidePieWithOffset = null;
            if (possibleSafePointInsidePieAngle.Count > 0)
            {
                var findClosestPointInTuple = possibleSafePointInsidePieAngle.Aggregate(
                    (i1, i2) =>
                        VectorHelper.DistanceSquared(i1.Item1, targetPosition) < VectorHelper.DistanceSquared(i2.Item1, targetPosition) ? i1 : i2);

                closestPointOutsidePieWithOffset = findClosestPointInTuple.Item1;
                closestPointOutsidePieWithOffset += findClosestPointInTuple.Item2; // offset
            }

            if (closestPointOutsidePieWithOffset == null)
            {
                Vector3 centerOfAOEWithOffsetFromAOE = aoeCircleController.Center + (-aoeCircleController.transform.forward * offsetFromAOE);
                return centerOfAOEWithOffsetFromAOE;
            }

            // var distanceSquaredToCenterOfAOE = VectorHelper.DistanceSquared(aoeCircleController.Center, aoeCircleController.Center);
            // var distanceSquaredToClosestPointOutsidePie = VectorHelper.DistanceSquared(closestPointOutsidePieWithOffset.Value, aoeCircleController.Center);
            var distanceSquaredToCenterOfAOE = VectorHelper.DistanceSquared(aoeCircleController.Center, targetPosition);
            var distanceSquaredToClosestPointOutsidePie = VectorHelper.DistanceSquared(closestPointOutsidePieWithOffset.Value, targetPosition);

            if (distanceSquaredToCenterOfAOE <= distanceSquaredToClosestPointOutsidePie)
                return aoeCircleController.GetSafeSpotAtCircleCenter(offsetFromAOE);

            return closestPointOutsidePieWithOffset; // + (directionToTarget * offsetFromAOE);
        }
        
        public static Vector3? GetNearestPositionSafeFromAOE(NpcAI npcAI, Vector3 creaturePosition, IEnumerable<AOECircleController> aoeCircleControllers, Vector3 targetPosition, float radiusOffset)
        {
            Vector3? safePosition;

            float offsetFromAOE = npcAI.EscapeFromAoeSafeAreaOffset;

            npcAI.ListSafePointsOutsideAOECircles.Clear();
            npcAI.ListMultipleAOECirclesIntersections.Clear();
            npcAI.ListDebugIntersectionLineWithPieLinesNotPieArea.Clear();
            npcAI.ListDebugIntersectionAOEPieLineWithSecondAOEPieLine.Clear();
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCircleRadius.Clear();
            npcAI.ListAOECirclePieLinesIntersectWithAnotherCirclePieLines.Clear();
            npcAI.ListAOECircleCenterWithOffset.Clear();

            var listAOECircleControllers = aoeCircleControllers.ToList();

            var possibleSafePoints = new List<Vector3>();

            if (listAOECircleControllers.Count == 1)
            {
                var aoeCircleController = listAOECircleControllers[0];

                possibleSafePoints.AddRange(AOECircleController.GetSafePointsForSingleAOE(npcAI, aoeCircleController, creaturePosition, targetPosition, offsetFromAOE));
            }
            else
            {
                // if (listAOECircleControllers.Count == 2)
                // {
                //     AOECircleController firstAOECircleController = listAOECircleControllers[0];
                //     AOECircleController secondAOECircleController = listAOECircleControllers[1];
                //
                //     Vector3[] safePointsBetweenTwoCircles1 = GetSafePointsBetweenTwoCircles(firstAOECircleController, secondAOECircleController,
                //         creaturePosition, targetPosition, offsetFromAOE, radiusOffset);
                //     possibleSafePoints.AddRange(safePointsBetweenTwoCircles1);
                //
                //     Vector3[] safePointsBetweenTwoCircles2 = GetSafePointsBetweenTwoCircles(secondAOECircleController, firstAOECircleController,
                //         creaturePosition, targetPosition, offsetFromAOE, radiusOffset);
                //     possibleSafePoints.AddRange(safePointsBetweenTwoCircles2);
                // }
                // else
                {
                    for (int i = 0; i < listAOECircleControllers.Count; i++)
                    {
                        if (i == listAOECircleControllers.Count - 1)
                        {
                            continue;
                        }

                        AOECircleController firstAOECircleController = listAOECircleControllers[i];

                        for (int j = i + 1; j < listAOECircleControllers.Count; j++)
                        {
                            AOECircleController secondAOECircleController = listAOECircleControllers[j];

                            var safePointsBetweenTwoCircles = AOECircleController.GetSafePointsBetweenTwoCircles(npcAI, firstAOECircleController, secondAOECircleController,
                                creaturePosition, targetPosition, offsetFromAOE, radiusOffset);

                            possibleSafePoints.AddRange(safePointsBetweenTwoCircles);
                        }
                    }
                }
            }

            // After that, remove from list all points which are not safe
            for (int i = possibleSafePoints.Count - 1; i >= 0; i--)
            {
                var possibleSafePoint = possibleSafePoints[i];

                foreach (var aoeCircleController in listAOECircleControllers)
                {
                    var checkPointInsideSpherePie = MathHelper.CheckPointInsideSpherePie(possibleSafePoint, aoeCircleController.Center, aoeCircleController.Radius, aoeCircleController.Angle,
                        aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward);

                    if (checkPointInsideSpherePie == MathHelper.ResultPointInsideSpherePie.InsideCirclePieArea)
                    {
                        possibleSafePoints.RemoveAt(i);
                        break;
                    }
                }
            }

            if (possibleSafePoints.Count == 0)
            {
                //throw new Exception($"({nameof(safePointsBetweenTwoCircles)}.Length == 0): Shouldn't be possible, right?");
                safePosition = null;
            }
            else if (possibleSafePoints.Count == 1)
            {
                safePosition = possibleSafePoints[0];
            }
            else
            {
                var nearestSafePositionToTarget = possibleSafePoints.Aggregate(
                    (i1, i2) =>
                        VectorHelper.DistanceSquared(i1, targetPosition) < VectorHelper.DistanceSquared(i2, targetPosition) ? i1 : i2);
                safePosition = nearestSafePositionToTarget;
            }

            return safePosition;
        }
        
        private static Vector3 GerNearestSafePointFromCircleAOE(Vector3 creaturePosition, AOECircleController aoeCircleController, Vector3 targetPosition, float offsetFromAOE, float radiusOffset)
        {
            // Для начала проверим что цель не стоит на краешке АОЕ. Для этого, выясним дистанции:
            // 1. От цели до ближайшего выхода из круга
            // 2. От цели до 1-й PIE линии
            // 3. От цели до 2-й PIE линии
            // 4. Выбираем возьмём меньшую из дистанций пункта 2 и 3.
            // Сравниваем что-то из этого с radiusOffset, и если оно меньше, то можем туда двигаться, если же оно больше - тогда смотрим дальше.

            Vector3 directionFromAOECenterToTarget = (targetPosition - aoeCircleController.Center).normalized;
            Vector3 safeSpotOutsideCircleWithOffset = aoeCircleController.Center + directionFromAOECenterToTarget * (aoeCircleController.Radius + offsetFromAOE);

            if (aoeCircleController.Angle >= 360)
                return safeSpotOutsideCircleWithOffset;

            float distanceSquaredToSafeSpotOutsideCircle = VectorHelper.DistanceSquared(safeSpotOutsideCircleWithOffset, targetPosition);

            var pieOuterPoints = aoeCircleController.GetPieOuterPoints();

            var pieLineIntersectionWithTarget1 = IntersectionResolver.IntersectionLineAndCircleAndOutsideOfAnotherCirclePieArea(aoeCircleController.Center, pieOuterPoints[0], targetPosition, radiusOffset,
                aoeCircleController.Center, aoeCircleController.Radius, aoeCircleController.transform.forward, aoeCircleController.ClockwiseRotation, aoeCircleController.Angle);
            var pieLineIntersectionWithTarget2 = IntersectionResolver.IntersectionLineAndCircleAndOutsideOfAnotherCirclePieArea(aoeCircleController.Center, pieOuterPoints[1], targetPosition, radiusOffset,
                aoeCircleController.Center, aoeCircleController.Radius, aoeCircleController.transform.forward, aoeCircleController.ClockwiseRotation, aoeCircleController.Angle);

            Vector3? pieLineClosestPoint1 = null;
            if (pieLineIntersectionWithTarget1 != null)
            {
                if (pieLineIntersectionWithTarget1.Length == 1)
                {
                    pieLineClosestPoint1 = pieLineIntersectionWithTarget1[0];
                }
                else
                {
                    // get point which is closest to "creaturePosition"
                    var distanceSquaredToPoint1 = VectorHelper.DistanceSquared(pieLineIntersectionWithTarget1[0], creaturePosition);
                    var distanceSquaredToPoint2 = VectorHelper.DistanceSquared(pieLineIntersectionWithTarget1[1], creaturePosition);
                    pieLineClosestPoint1 = distanceSquaredToPoint1 < distanceSquaredToPoint2 ? pieLineIntersectionWithTarget1[0] : pieLineIntersectionWithTarget1[1];
                }
            }

            Vector3? pieLineClosestPoint2 = null;
            if (pieLineIntersectionWithTarget2 != null)
            {
                if (pieLineIntersectionWithTarget2.Length == 1)
                {
                    pieLineClosestPoint2 = pieLineIntersectionWithTarget2[0];
                }
                else
                {
                    // get point which is closest to "creaturePosition"
                    var distanceSquaredToPoint1 = VectorHelper.DistanceSquared(pieLineIntersectionWithTarget2[0], creaturePosition);
                    var distanceSquaredToPoint2 = VectorHelper.DistanceSquared(pieLineIntersectionWithTarget2[1], creaturePosition);
                    pieLineClosestPoint2 = distanceSquaredToPoint1 < distanceSquaredToPoint2 ? pieLineIntersectionWithTarget2[0] : pieLineIntersectionWithTarget2[1];
                }
            }

            var lineFromAOECenterToPiePoint1 = (aoeCircleController.Center - pieOuterPoints[0]).normalized;
            var lineFromAOECenterToPiePoint2 = (aoeCircleController.Center - pieOuterPoints[1]).normalized;

            // we know that pieLineClosesPoint is nice, since it is located on PieLine (which formed by PieAngle)
            // When we later will add offset to it, if will be almost always a safe point for this AOE.
            if (pieLineClosestPoint1 != null && pieLineClosestPoint2 != null)
            {
                var distanceSquaredToPoint1 = VectorHelper.DistanceSquared(pieLineClosestPoint1.Value, creaturePosition);
                var distanceSquaredToPoint2 = VectorHelper.DistanceSquared(pieLineClosestPoint2.Value, creaturePosition);

                if (distanceSquaredToPoint1 < distanceSquaredToSafeSpotOutsideCircle || distanceSquaredToPoint2 < distanceSquaredToSafeSpotOutsideCircle)
                {
                    var pieLineClosestPoint1WithOffset = pieLineClosestPoint1.Value + Vector3.Cross(lineFromAOECenterToPiePoint1, -Vector3.up).normalized * offsetFromAOE;
                    var pieLineClosestPoint2WithOffset = pieLineClosestPoint2.Value + Vector3.Cross(lineFromAOECenterToPiePoint2, Vector3.up).normalized * offsetFromAOE;

                    if (distanceSquaredToPoint1 < distanceSquaredToPoint2)
                    {
                        return pieLineClosestPoint1WithOffset;
                    }
                    else
                    {
                        return pieLineClosestPoint2WithOffset;
                    }
                }
            }

            if (pieLineClosestPoint1 != null)
            {
                Vector3 ret = pieLineClosestPoint1.Value + Vector3.Cross(lineFromAOECenterToPiePoint1, -Vector3.up).normalized * offsetFromAOE;
                return ret;
            }

            if (pieLineClosestPoint2 != null)
            {
                Vector3 ret = pieLineClosestPoint2.Value + Vector3.Cross(lineFromAOECenterToPiePoint2, Vector3.up).normalized * offsetFromAOE;
                return ret;
            }

            // требуется проверить, находится ли "targetPosition" внутри круга, если нет:
            // НЕВЕРНО: тогда сейф спот будет на том же месте где и "targetPosition"
            // ВЕРНО: сейф спот будет находиться на дистанции радиуса круга по направлению к цели

            // Если же существо всё таки внутри круга, то проверяем в какой части круга оно стоит
            // 1. Если существо в стороне круга который входит в PIE-зону ("опасная" зона АОЕ), то ищем ближайший сейф-спорт снаружи этой зоны, а затем также пункт 2, после чего выбираем тот сейф спорт, где дистанция до "targetPosition" меньше
            // 2. Если существо в "безопасной" стороне круга, ищем сейф-спот по формуле "LineLineIntersection"

            MathHelper.ResultPointInsideSpherePie checkPointInsideSpherePie = MathHelper.CheckPointInsideSpherePie(targetPosition, aoeCircleController.Center, aoeCircleController.Radius,
                aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward); //, radiusOffset);

            if (checkPointInsideSpherePie == MathHelper.ResultPointInsideSpherePie.NotInsideSphere)
            {
                // Debug.LogError($"{nameof(MathHelper.ResultPointInsideSpherePie.NotInsideSphere)} - something like this is really possible?");
                // return targetPosition;
                return safeSpotOutsideCircleWithOffset;
            }

            // Vector3 directionToSafeArea = (creaturePosition - aoeCircleController.Center).normalized;
            // Vector3 safePosition = aoeCircleController.Center + directionToSafeArea * (aoeCircleController.Radius + offsetFromAOE);
            // return safePosition;

            Vector3? nearestSafePointFromCirclePieAOE = AOECircleController.GerNearestSafePointFromCirclePieAOE(creaturePosition, aoeCircleController, targetPosition, offsetFromAOE, radiusOffset);
            if (nearestSafePointFromCirclePieAOE == null)
            {
                return safeSpotOutsideCircleWithOffset;
            }

            float distanceSquaredToNearestSafePointFromCirclePieAOE = VectorHelper.DistanceSquared(nearestSafePointFromCirclePieAOE.Value, targetPosition);

            if (distanceSquaredToSafeSpotOutsideCircle < distanceSquaredToNearestSafePointFromCirclePieAOE)
            {
                // Debug.Log($"Shorter path to point outside circle");
                return safeSpotOutsideCircleWithOffset;
            }
            else
            {
                // Debug.Log($"Shorter path to point inside circle, outside Pie AOE");
                return nearestSafePointFromCirclePieAOE.Value;
            }
        }
        
        public static bool IsPointInsideAOECircleController(Vector3 point, AOECircleController aoeCircleController, float creaturePositionPossibleOffset)
        {
            var checkPointInsideSpherePie = MathHelper.CheckPointInsideSpherePie(point, aoeCircleController.Center, aoeCircleController.Radius,
                aoeCircleController.Angle, aoeCircleController.ClockwiseRotation, aoeCircleController.transform.forward); //, creaturePositionPossibleOffset);

            if (checkPointInsideSpherePie == MathHelper.ResultPointInsideSpherePie.InsideCirclePieArea)
            {
                return true;
            }

            return false;
        }

        public static List<AOECircleController> GetAOEsCreatureIn(Vector3 creaturePosition, IEnumerable<AOECircleController> aoeCircleControllers, bool isCatchAOEByCapsuleCollider, float creaturePositionPossibleOffset)
        {
            var ret = new List<AOECircleController>();

            foreach (var aoeCircleController in aoeCircleControllers)
            {
                if (IsCreatureInsideAOE(creaturePosition, aoeCircleController, isCatchAOEByCapsuleCollider, creaturePositionPossibleOffset))
                {
                    ret.Add(aoeCircleController);
                }
            }

            return ret;
        }

        public static bool IsCreatureInsideAOEs(Vector3 creaturePosition, IEnumerable<AOECircleController> aoeCircleControllers, bool isCatchAOEByCapsuleCollider, float creaturePositionPossibleOffset)
        {
            foreach (var aoeCircleController in aoeCircleControllers)
            {
                if (IsCreatureInsideAOE(creaturePosition, aoeCircleController, isCatchAOEByCapsuleCollider, creaturePositionPossibleOffset))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCreatureInsideAOE(Vector3 creaturePosition, AOECircleController aoeCircleController, bool isCatchAOEByCapsuleCollider, float creaturePositionPossibleOffset)
        {
            if (isCatchAOEByCapsuleCollider)
            {
                // throw new NotImplementedException();
                Debug.LogError($"{nameof(isCatchAOEByCapsuleCollider)} logic is not implemented.");
            }
            else
            {
                if (IsPointInsideAOECircleController(creaturePosition, aoeCircleController, creaturePositionPossibleOffset))
                {
                    return true;
                }
            }

            return false;
        }
    }
}