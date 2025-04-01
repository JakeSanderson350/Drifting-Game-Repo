using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class Temp : MonoBehaviour
{
    [Header("Spline Mesh Info")]
    public SplineContainer splineContainer;
    public GameObject splineObj;
    public List<BezierKnot> totalKnots;

    [Space(10)]
    [Header("Step Values")]
    public float minStepX; // Smallest X step
    public float maxStepX; // Largest X step
    [Tooltip("The space between the edge of the cube and the road")]
    public float spaceValue;

    [Space(10)]
    [Header("Knot Values")]
    public float3 tangentIn = new float3(-0.4714046);
    public float3 tangentOut = new float3(0.4714046);

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

        // Set spline as child of cube
        splineContainer.transform.SetParent(cube.transform, worldPositionStays: true);
        Bounds bounds = cube.GetComponent<Renderer>().bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        float minZBound = min.z + spaceValue;
        float maxZBound = max.z - spaceValue;
        float topY = bounds.max.y + 0.01f;

        totalKnots = new List<BezierKnot>();

        // First knot exactly on the edge with X-axis perpendicular tangent
        float randomZ = Random.Range(minZBound, maxZBound);
        Vector3 startPos = new Vector3(min.x, topY, randomZ);

        // Simple X-axis tangent - positive for out, negative for in
        Vector3 xAxisTangent = new Vector3(1.0f, 0, 0);

        // Add first knot with X-axis tangent
        totalKnots.Add(new BezierKnot(startPos, -xAxisTangent, xAxisTangent, quaternion.identity));

        // Generate the rest of the spline normally
        float xPosition = min.x + UnityEngine.Random.Range(minStepX, maxStepX);
        while (xPosition < max.x)
        {
            randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
            Vector3 knotPosition = new Vector3(xPosition, topY, randomZ);
            totalKnots.Add(new BezierKnot(knotPosition, tangentIn, tangentOut, quaternion.identity));

            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);
            if (nextX > max.x + 0.75f)
            {
                nextX = max.x;
                randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
                totalKnots.Add(new BezierKnot(new Vector3(nextX, topY, randomZ), float3.zero, float3.zero, quaternion.identity));
                break;
            }
            xPosition = nextX;
        }

        splineContainer.Spline = new Spline(totalKnots, closed: false);

        // Critical: Don't auto-smooth the first knot!
        for (int i = 1; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
        splineContainer.Spline.SetTangentMode(0, TangentMode.Mirrored);

        splineObj.AddComponent<LoftRoadBehaviour>();
    }
    public Vector3 lastKnotPos()
    {
        return totalKnots[totalKnots.Count - 1].Position;
    }
    public float lastKnotRot()
    {
        Vector3 endTangent = splineContainer.Spline.EvaluateTangent(1f);
        Quaternion trackEndRotation = Quaternion.LookRotation(endTangent);
        float yaw = trackEndRotation.eulerAngles.y;
        return yaw;
    }
}
