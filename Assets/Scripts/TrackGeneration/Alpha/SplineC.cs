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
    [Tooltip("Max angle allowed between points")]
    public float maxAngle;
    [Tooltip("Number of attempts to get a non harsh angle between knots")]
    const int maxAttempts = 20;

    [Space(10)]
    [Header("Knot Values")]
    public float3 tangentIn    = new float3(-0.4714046);
    public float3 tangentOut   = new float3(0.4714046);

    public GameObject Init()
    { 
        splineObj = new GameObject("Procedural Spline");
        splineContainer = splineObj.AddComponent<SplineContainer>();
        splineObj.tag = "Spline";
        return splineObj;
    }

    public void GenerateKnots(GameObject cube, Quaternion lastKnotRot)
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
        //0.01 is added so spline sits on top of cube
        float topY = bounds.max.y + 0.01f;  

        totalKnots = new List<BezierKnot>();
        Vector3 rightDirection = cube.transform.right;

        //create first knot perpendicular to edge add to list w/ perpendicular tangent
        float randomZ = Random.Range(minZBound, maxZBound);
        Vector3 startPos = new Vector3(min.x, topY, randomZ);
        totalKnots.Add(new BezierKnot(startPos, -rightDirection, rightDirection, lastKnotRot));

        Vector3 prevKnotPos = startPos;
        Vector3 prevDirection = rightDirection;

        //generate rest of knots along a random inc from min.x to max.x
        float xPosition = min.x + UnityEngine.Random.Range(minStepX, maxStepX);

        while (xPosition < max.x)
        {
            Vector3 knotPosition = Vector3.zero;

            //try to get a non harsh angle between knots
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                //generate position
                randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
                knotPosition = new Vector3(xPosition, topY, randomZ);

                //calc direction
                Vector3 newDirection = (knotPosition - prevKnotPos).normalized;

                //calc angle between
                float angle = Vector3.Angle(prevDirection, newDirection);

                //if less than maxAngle hurray! point is valid
                if (angle < maxAngle)
                {
                    prevDirection = newDirection; 
                    break;
                }
            }

            //add the knot
            totalKnots.Add(new BezierKnot(knotPosition, tangentIn, tangentOut, quaternion.identity));
            prevKnotPos = knotPosition;

            //calculate next knot position
            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);

            //if position would be more than max.x, set equal to max.x
            if (nextX > max.x + 1.5f)
            {
                //checking angle constraints
                Vector3 finalPos = Vector3.zero;
                Vector3 finalDirection = Vector3.zero;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    //generate position
                    randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
                    finalPos = new Vector3(max.x, topY, randomZ);

                    //calc final direction
                    finalDirection = (finalPos - prevKnotPos).normalized;

                    //calc angle between
                    float angle = Vector3.Angle(prevDirection, finalDirection);

                    //if less than maxAngle hurray! point is valid
                    if (angle < maxAngle)
                    {
                        break;
                    }
                }

                //add final knot
                totalKnots.Add(new BezierKnot(finalPos, -rightDirection, rightDirection, quaternion.identity));
                break;
            }

            xPosition = nextX;
        }

        splineContainer.Spline = new Spline(totalKnots, closed: false);

        //important: DONT autosmooth first knot, will mess up tangents
        for (int i = 1; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }

        //set first as mirrored
        splineContainer.Spline.SetTangentMode(0, TangentMode.Mirrored);

        splineObj.AddComponent<LoftRoadBehaviour>();
    }

    public Vector3 firstKnotPos(GameObject givenSpline)
    {
        Vector3 pos = givenSpline.GetComponent<SplineContainer>().Spline.EvaluatePosition(0f);
        pos = givenSpline.transform.TransformPoint(pos);
        return pos;
    }
    public Vector3 lastKnotPos(GameObject givenSpline)
    {
        Vector3 pos = givenSpline.GetComponent<SplineContainer>().Spline.EvaluatePosition(1f);
        pos = givenSpline.transform.TransformPoint(pos);
        return pos; 
    }
    public Quaternion lastKnotRot(GameObject givenSpline)
    {
        Vector3 endTangent = givenSpline.GetComponent<SplineContainer>().Spline.EvaluateTangent(1f);
        Vector3 endUpVector = givenSpline.GetComponent<SplineContainer>().Spline.EvaluateUpVector(1f);
        Quaternion quaternion = Quaternion.LookRotation(endTangent, endUpVector);

        Quaternion correction = Quaternion.Euler(0, -90, 0);
        quaternion = quaternion * correction;
        return quaternion;
    }
}
