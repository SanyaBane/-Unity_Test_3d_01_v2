using System;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.Managers;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.AreaOfEffects
{
    public class AOECircleGraphUpdateObject : GraphUpdateObject
    {
        private static float tileSideWidth = AstarPath.active.data.recastGraph.editorTileSize * AstarPath.active.data.recastGraph.cellSize;
        private static float tileSideDiagonal = Mathf.Sqrt(2) * tileSideWidth;
        private static float tileSideDiagonalMultiply2 = tileSideDiagonal * 2;
        private static Vector2 tileCenter = new Vector2(tileSideWidth / 3, tileSideWidth / 3);
        private static float distanceFromCenterToPoint = Vector2.Distance(tileCenter, new Vector2(0, tileSideWidth));

        private readonly float _sphereRadius;

        private readonly AOECircleController _aoeCircleController;

        private readonly Vector3 _forward;

        public AOECircleGraphUpdateObject(AOECircleController aoeCircleController, Vector3 forward, Bounds bounds, float sphereRadius)
        {
            this._aoeCircleController = aoeCircleController;
            this._forward = forward;
            this.bounds = bounds;
            this._sphereRadius = sphereRadius;

            this.updatePhysics = false;
            this.trackChangedNodes = true;
        }

        public override void Apply(GraphNode node)
        {
            // Debug.Log($"NoPathGraphUpdateObject Apply()");

            try
            {
                var checkPointInsideSpherePie1 = MathHelper.CheckPointInsideSpherePie((Vector3) node.position, bounds.center, _sphereRadius + distanceFromCenterToPoint,
                    _aoeCircleController.Angle, _aoeCircleController.ClockwiseRotation, _forward);

                if (checkPointInsideSpherePie1 == MathHelper.ResultPointInsideSpherePie.InsideCirclePieArea)
                {
                    node.Tag |= GetNodeTag((Vector3) node.position);
                    GameManager.Instance.AOEManager.AddGraphNodeForTesting(node);
                    return;
                }

                var checkPointInsideSpherePieWithOffset = MathHelper.CheckPointInsideSpherePie((Vector3) node.position, bounds.center + (-_forward * distanceFromCenterToPoint), _sphereRadius + distanceFromCenterToPoint,
                    _aoeCircleController.Angle, _aoeCircleController.ClockwiseRotation, _forward);

                if (checkPointInsideSpherePieWithOffset == MathHelper.ResultPointInsideSpherePie.InsideCirclePieArea)
                {
                    node.Tag |= GetNodeTag((Vector3) node.position);
                    GameManager.Instance.AOEManager.AddGraphNodeForTesting(node);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public uint GetNodeTag(Vector3 nodePosition)
        {
            // var distanceSquaredFromAOECenterToNodePosition = VectorHelper.DistanceSquared(this.bounds.center, nodePosition);
            //
            // if (distanceSquaredFromAOECenterToNodePosition <= tileSideDiagonal * tileSideDiagonal)
            // {
            //     return AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_1;
            // }
            //
            // if (distanceSquaredFromAOECenterToNodePosition <= tileSideDiagonalMultiply2 * tileSideDiagonalMultiply2)
            // {
            //     return AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_2;
            // }

            return AOECircleController.PARTY_MEMBER_NODE_TAG_ENEMY_AOE_3;
        }
    }
}