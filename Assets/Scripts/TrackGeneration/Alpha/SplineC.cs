using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Splines.Examples;
using Unity.VisualScripting;
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
    [Tooltip("The space between the edge of the cube and the road")]
    public float spaceValue;

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
        Bounds bounds = cube.GetComponent<Renderer>().bounds;

        //set min and max knot spawning values
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        //leaving some space on left and right of track
        float minZBound = min.z + spaceValue;
        float maxZBound = max.z - spaceValue;
        float topY = bounds.max.y + 0.01f;

        totalKnots = new List<BezierKnot>();

        //create first knot perpendicular to edge
        float randomZ = Random.Range(minZBound, maxZBound);
        Vector3 startPos = new Vector3(min.x, topY, randomZ);
        Vector3 xAxisTangent = new Vector3(1.0f, 0, 0);

        //add first knot to list w/ perpendicular tangent
        totalKnots.Add(new BezierKnot(startPos, -xAxisTangent, xAxisTangent, quaternion.identity));

        //generate rest of knots along a random inc from min.x to max.x
        float xPosition = min.x + UnityEngine.Random.Range(minStepX, maxStepX);

        while (xPosition < max.x)
        {
            //gen position
            randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
            Vector3 knotPosition = new Vector3(xPosition, topY, randomZ);

            //add knot
            totalKnots.Add(new BezierKnot(knotPosition, tangentIn, tangentOut, quaternion.identity));

            //step
            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);

            //if next knot is going to be over the edge put it at max.x
            if (nextX > max.x + 0.75f)
            {
                nextX = max.x;
                randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
                totalKnots.Add(new BezierKnot(new Vector3(nextX, topY, randomZ), float3.zero, float3.zero, quaternion.identity));

                Debug.Log("After Creation: " + totalKnots[totalKnots.Count - 1].Rotation);
                break;
            }

            xPosition = nextX;
        }

        splineContainer.Spline = new Spline(totalKnots, closed: false);

        //important: DONT autosmooth first and last knot, will mess up tangents
        for (int i = 1; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
        Quaternion rotation = Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(1f));
        Debug.Log("After Smoothing: " + rotation.eulerAngles);

        //set first as mirrored
        splineContainer.Spline.SetTangentMode(0, TangentMode.Mirrored);

        splineContainer.transform.SetParent(cube.transform, worldPositionStays: true);

        Quaternion rotation2 = Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(1f));
        Debug.Log("After Parenting: " + rotation2.eulerAngles);

        splineObj.AddComponent<LoftRoadBehaviour>();
    }
    public Vector3 lastKnotPos()
    {
        return totalKnots[totalKnots.Count - 1].Position;
    }
    public Quaternion lastKnotRot()
    {
        Quaternion rotation = Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(1f));
        Debug.Log("Before sending: " + rotation.eulerAngles);
        return totalKnots[totalKnots.Count - 1].Rotation;
    }
}
