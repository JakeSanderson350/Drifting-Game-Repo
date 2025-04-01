using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class SplineC: MonoBehaviour
{
    [Header("Spline Mesh Info")]
    public SplineContainer splineContainer;
    public GameObject splineObj;
    public List<BezierKnot> totalKnots;

    [Space(10)]
    [Header("Step Values")]
    public float minStepX; // Smallest X step
    public float maxStepX; // Largest X step

    [Space(10)]
    [Header("Knot Values")]
    public float3 tangentIn    = new float3(-0.4714046);
    public float3 tangentOut   = new float3(0.4714046);

    public void Init()
    { 
        splineObj = new GameObject("Procedural Spline");
        splineContainer = splineObj.AddComponent<SplineContainer>();
        splineObj.tag = "Spline";
    }

    public void AttachCube(GameObject cube)
    {
        if (cube == null || splineContainer == null)
        {
            Debug.LogError("Cube or SplineContainer is not assigned!");
            return;
        }

        //set spline as child of cube
        splineContainer.transform.SetParent(cube.transform, worldPositionStays: true);

        Bounds bounds = cube.GetComponent<Renderer>().bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        //so spline sits on top of mesh
        float topY = bounds.max.y + 0.01f;

        float xPosition = min.x;

        totalKnots = new List<BezierKnot>();

        while (xPosition < max.x)
        {
            float randomZ = UnityEngine.Random.Range(min.z, max.z);
            Vector3 knotPosition = new Vector3(xPosition, topY, randomZ);

            totalKnots.Add(new BezierKnot(knotPosition, tangentIn, tangentOut, quaternion.identity));

            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);

            if (nextX >= max.x)
            {
                nextX = max.x;
                randomZ = UnityEngine.Random.Range(min.z, max.z); 
                totalKnots.Add(new BezierKnot(new Vector3(nextX, topY, randomZ), float3.zero, float3.zero, quaternion.identity));
                break;
            }

            xPosition = nextX;
        }

        splineContainer.Spline = new Spline(totalKnots, closed: false);

        //auto smooth tangents
        for (int i = 0; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }

        splineObj.AddComponent<LoftRoadBehaviour>();
    }

    public Vector3 lastKnotPos()
    {
        return totalKnots[totalKnots.Count - 1].Position;
    }
}
