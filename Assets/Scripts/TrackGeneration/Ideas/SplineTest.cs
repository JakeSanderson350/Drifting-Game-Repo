using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [Header("Spline Info")]
    public SplineContainer splineContainer;

    [Header("Knot Positions")]
    public float3 p1 = new float3(1.0f, 0.0f, 1.0f);
    public float3 p2 = new float3(1.0f, 0.0f, -1.0f);
    public float3 p3 = new float3(-1.0f, 0.0f, -1.0f);
    public float3 p4 = new float3(-1.0f, 0.0f, 1.0f);

    [Header("Knot Tangents")]
    public float3 tangentIn    = new float3(-0.4714046);
    public float3 tangentOut   = new float3(0.4714046);

    void Start()
    {
        UpdateSpline();
    }
    private void OnValidate()
    {
        UpdateSpline();
    }

    public void UpdateSpline()
    {
        List<BezierKnot> totalKnots = new List<BezierKnot>
        {
            new BezierKnot(p1, tangentIn, tangentOut, quaternion.identity),
            new BezierKnot(p2, tangentIn, tangentOut, quaternion.identity),
            new BezierKnot(p3, tangentIn, tangentOut, quaternion.identity),
            new BezierKnot(p4, tangentIn, tangentOut, quaternion.identity),
        };

        splineContainer.Spline = new Spline(totalKnots, closed: true);

        for (int i = 0; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
    }
}
