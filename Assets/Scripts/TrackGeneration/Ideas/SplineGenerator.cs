using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class SplineGenerator : MonoBehaviour
{
    //need to create spline class for generated splines
    public class SplineH
    {

    }

    [Header("Spline Info")]
    public SplineContainer splineContainer;

    [Header("Mesh Info")]
    public GameObject splineObj;
    public GameObject myCell;

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
        splineObj = new GameObject("Procedural Spline");
        splineContainer = splineObj.AddComponent<SplineContainer>();
        splineObj.tag = "Spline";

        UpdateSpline();
    }
    public void OnValidate()
    {
        UpdateSpline();
    }

    public void UpdateSpline()
    {
        //need to convert to world space then calculate bounds
/*        Bounds bounds = myCell.GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 center = myCell.GetComponent<Mesh>().bounds.center;

        float x = Random.Range(center.x - bounds.x, center.x + bounds.x);
        float y = Random.Range(center.y - bounds.y, center.y + bounds.y);
        float z = Random.Range(center.z - bounds.z, center.z + bounds.z);
        p1 = new float3(x, y, z);*/

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

    public void SetCell(GameObject cell)
    {
        myCell = cell;
    }
}
