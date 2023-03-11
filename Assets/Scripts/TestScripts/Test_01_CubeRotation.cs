using UnityEngine;

public class Test_01_CubeRotation : MonoBehaviour {

    public float angleInDegree;

#pragma warning disable 0414    // suppress value not used warning
    [SerializeField]
    float angleInRadians;
#pragma warning restore 0414    // restore value not used warning

    public Vector3 axisRotation;

    [SerializeField]
    public Vector3 axisRotationNormalized;

    Quaternion q;

	void Start () {
		
	}
	
	void Update () {
        angleInRadians = angleInDegree * Mathf.Deg2Rad;
        axisRotationNormalized = axisRotation.normalized;

        //q = new Quaternion(Mathf.Sin(angleInRadians / 2) * axisRotationNormalized.x, Mathf.Sin(angleInRadians / 2) * axisRotationNormalized.y, Mathf.Sin(angleInRadians / 2) * axisRotationNormalized.z, Mathf.Cos(angleInRadians / 2));
        q = Quaternion.AngleAxis(angleInDegree, axisRotationNormalized);

        transform.rotation = transform.rotation * q;

        //Debug.DrawRay(new Vector3(0, 0, 0), axisRotation);
        Debug.DrawRay(new Vector3(0, 0, 0), axisRotationNormalized);
        Debug.DrawRay(new Vector3(0, 0, 0), -axisRotationNormalized);
    }
}
