using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [SerializeField]
    private SplineContainer splineContainer;
    [SerializeField]
    private int splineIndex;
    [SerializeField]
    [Range(0f, 1f)]
    private float time;
    [SerializeField]
    private float width = 3f;

    float3 p1;
    float3 p2;

    float3 position;
    float3 tangent;
    float3 upVector;

    void Update()
    {
        splineContainer.Evaluate(splineIndex, time, out position, out tangent, out upVector);

        //tangent is forward direction of travel along the spline
        //find right and left direction based on this
        float3 right = Vector3.Cross(tangent, upVector).normalized;

        p1 = position + (right * width);
        p2 = position + (-right * width);
    }

    private void OnDrawGizmos()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.SphereHandleCap(0, position, Quaternion.identity, 1f, EventType.Repaint);
    }
}
