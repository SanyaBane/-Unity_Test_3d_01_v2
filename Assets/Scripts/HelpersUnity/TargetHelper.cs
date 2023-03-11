using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class TargetHelper
    {
        public static float DistanceBetweenCreatureColliders(IBaseCreature baseCreature1, IBaseCreature baseCreature2)
        {
            float distanceBetweenPivots = Vector3.Distance(baseCreature1.GetRootObjectTransform().position, baseCreature2.GetRootObjectTransform().position);
            return DistanceBetweenCreatureColliders(baseCreature1, baseCreature2, distanceBetweenPivots);
        }

        public static float DistanceBetweenCreatureColliders(IBaseCreature baseCreature1, IBaseCreature baseCreature2, float distanceBetweenPivots)
        {
            float ret = distanceBetweenPivots;
            ret -= baseCreature1.CreatureMeasures.Radius;
            ret -= baseCreature2.CreatureMeasures.Radius;

            if (ret < 0)
                ret = 0;

            return ret;
        }

        public static bool IsNotBlockedByTerrain(IBaseCreature baseCreature1, IBaseCreature baseCreature2, bool isDrawRays)
        {
            var ret = IsNotBlockedByTerrain(
                baseCreature1.GetRootObjectTransform().position, baseCreature1.CreatureMeasures.Height,
                baseCreature2.GetRootObjectTransform().position, baseCreature2.CreatureMeasures.Height,
                isDrawRays);
            return ret;
        }

        public static bool IsNotBlockedByTerrain(Vector3 creature1Position, float creature1Height, Vector3 creature2Position, float creature2Height, bool isDrawRays)
        {
            Color drawLineColorVisible = Color.green;
            Color drawLineColorNotVisible = Color.red;

            float heightReducerForVisibilityCheck = 0.8f;

            creature1Height *= heightReducerForVisibilityCheck;
            creature2Height *= heightReducerForVisibilityCheck;

            int numberOfRays = 3;

            bool[] linecastResult = new bool[numberOfRays];

            for (int i = 0; i < numberOfRays; i++)
            {
                var rayStartPos = creature1Position + new Vector3(0, creature1Height / (numberOfRays - 1) * i, 0);
                var rayEndPos = creature2Position + new Vector3(0, creature2Height / (numberOfRays - 1) * i, 0);

                Color drawLineColor;
                int groundLayerIndex = LayerMask.NameToLayer(LayerManager.LAYER_NAME_GROUND);
                if (Physics.Linecast(rayStartPos, rayEndPos, 1 << groundLayerIndex))
                {
                    linecastResult[i] = false;
                    drawLineColor = drawLineColorNotVisible;
                }
                else
                {
                    linecastResult[i] = true;
                    drawLineColor = drawLineColorVisible;
                }

                if (isDrawRays)
                    Debug.DrawLine(rayStartPos, rayEndPos, drawLineColor);
            }

            for (int i = 0; i < numberOfRays; i++)
            {
                if (linecastResult[i])
                    return true;
            }

            return false;
        }
    }
}
