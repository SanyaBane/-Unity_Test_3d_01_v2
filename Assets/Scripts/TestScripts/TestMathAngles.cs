using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMathAngles : MonoBehaviour
{
    //[SerializeField] private float _cosResult = 0;
    //[SerializeField] private float _sinResult = 0;

    [Range(0f, Mathf.PI * 2)]
    public float Input = Mathf.PI;

    public Transform TargetRotateAround;
    public Vector3 TargetOffset = Vector3.zero;

    void Start()
    {
    }

    void Update()
    {
        float _cosResult = Mathf.Cos(Input);
        float _sinResult = Mathf.Sin(Input);

        transform.position = new Vector3(
            TargetRotateAround.transform.position.x + TargetOffset.x + _cosResult,
            TargetRotateAround.transform.position.y + TargetOffset.y,
            TargetRotateAround.transform.position.z + TargetOffset.z + _sinResult);
    }
}
