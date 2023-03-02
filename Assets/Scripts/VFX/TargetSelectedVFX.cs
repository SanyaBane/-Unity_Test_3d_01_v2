using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.VFX
{
    public class TargetSelectedVFX : MonoBehaviour
    {
        // when "CreatureMeasures.radius" equal "0.45", then "xy-scale" equal "1.0"
        private const float BASIC_SIZE = 0.45f;

        public GameObject CirclesContainer;
        public GameObject CirclesTransform;
        public GameObject ArrowTransform;

        public float CiclesScaleOnSpawn = 1.5f;
        public float CiclesScaleNormal = 1.0f;
        public float CiclesScalingSpeed = 0.05f;

        public float ArrowScaleOnSpawn = 1.2f;
        public float ArrowScaleNormal = 0.65f;
        public float ArrowScalingSpeed = 0.05f;

        public bool TargetHasFrontAndBack { get; private set; }

        public void SetupAndStart(IBaseCreature baseCreature, Color color)
        {
            this.TargetHasFrontAndBack = baseCreature.ITargetable.HasFrontAndBack;

            float scale = baseCreature.CreatureMeasures.Radius / BASIC_SIZE;
            this.transform.localScale = new Vector3(scale, scale, scale);

            var circlesTransformRender = CirclesTransform.GetComponent<Renderer>();
            circlesTransformRender.material.SetColor("_Color", color);

            var arrowTransformRender = ArrowTransform.GetComponent<Renderer>();
            arrowTransformRender.material.SetColor("_Color", color);

            CirclesContainer.transform.localScale = new Vector3(CiclesScaleOnSpawn, CiclesScaleOnSpawn, CiclesScaleOnSpawn);
            ArrowTransform.transform.localScale = new Vector3(ArrowScaleOnSpawn, ArrowScaleOnSpawn, ArrowScaleOnSpawn);

            StartCoroutine(SpawnEffect(CirclesContainer.transform, CiclesScaleNormal, CiclesScalingSpeed));

            if (TargetHasFrontAndBack)
            {
                StartCoroutine(SpawnEffect(ArrowTransform.transform, ArrowScaleNormal, ArrowScalingSpeed));
            }
        }

        private IEnumerator SpawnEffect(Transform transform, float scaleGoal, float changeScaleSpeed)
        {
            while (transform.localScale.x != scaleGoal)
            {
                float speedVal = changeScaleSpeed;

                if (transform.localScale.x < scaleGoal)
                {
                    transform.localScale = new Vector3(
                        Mathf.Clamp(transform.localScale.x + speedVal, transform.localScale.x, scaleGoal),
                        Mathf.Clamp(transform.localScale.y + speedVal, transform.localScale.y, scaleGoal),
                        Mathf.Clamp(transform.localScale.z + speedVal, transform.localScale.z, scaleGoal));
                }
                else
                {
                    speedVal = speedVal * -1;

                    transform.localScale = new Vector3(
                        Mathf.Clamp(transform.localScale.x + speedVal, scaleGoal, transform.localScale.x),
                        Mathf.Clamp(transform.localScale.y + speedVal, scaleGoal, transform.localScale.y),
                        Mathf.Clamp(transform.localScale.z + speedVal, scaleGoal, transform.localScale.z));
                }

                yield return null;
            }
        }


    }
}
