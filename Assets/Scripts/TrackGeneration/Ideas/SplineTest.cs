using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [Header("Spline Info")]
    public SplineContainer splineContainer;
    public Spline spline;

    public BezierKnot knot1;
    public BezierKnot knot2;
    public BezierKnot knot3;
    public BezierKnot knot4;

    public BezierCurve curve1;
    public BezierCurve curve2;
    public BezierCurve curve3;
    public BezierCurve curve4;

    private float3 p1 = new float3(5.0f, 1.0f, 5.0f);
    private float3 p2 = new float3(5.0f, 1.0f, -5.0f);
    private float3 p3 = new float3(-5.0f, 1.0f, -5.0f);
    private float3 p4 = new float3(-5.0f, 1.0f, 5.0f);

    private float3 tangentIn    = new float3(-2.5);
    private float3 tangentOut   = new float3(2.5);

    void Start()
    {
        spline = splineContainer.spline;

        //BezierKnot(float3 positon, float3 tangentIn, float3 tangentOut, quaternion rotation)
        knot1 = new BezierKnot(p1, tangentIn, tangentOut, new Quaternion(9, 135, 2, 1));
        knot2 = new BezierKnot(p2, tangentIn, tangentOut, new Quaternion(1, 225, 359, 1));
        knot3 = new BezierKnot(p3, tangentIn, tangentOut, new Quaternion(0, 315, 0, 1));
        knot4 = new BezierKnot(p4, tangentIn, tangentOut, new Quaternion(0, 45, 1.5f, 1));

        spline.Add(knot1);
        spline.Add(knot2);
        spline.Add(knot3);
        spline.Add(knot4);
    
        curve1 = new BezierCurve(p1, p2);
        curve2 = new BezierCurve(p2, p3);
        curve3 = new BezierCurve(p3, p4);
        curve4 = new BezierCurve(p4, p1);

        //spline.closed.set = true;
    }
}
