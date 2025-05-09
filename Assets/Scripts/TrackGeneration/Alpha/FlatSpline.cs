using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class FlatSpline : MonoBehaviour
{
    [Header("Spline Mesh Info")]
    public SplineContainer splineContainer; //container holds spline data
    public GameObject splineObj;            //object the spline container is attached to
    public List<BezierKnot> totalKnots;     //collection of knots in current spline

    [Space(10)]
    [Header("Step Values")]
    public float minStepX;                  //min distance between knots on x axis
    public float maxStepX;                  //max distance ....
    public float spaceValue;                //buffer space from edge of cube
    public float maxAngle;                  //max angle allowed between knots (to prevent sharp turns)

    const int maxAttempts = 20;             //num of temps to find a valid angle

    //<summary> initalizes a new spline gameobject with components
    //<returns> the newly created spline object
    public GameObject Init()
    {
        splineObj = new GameObject("Procedural Spline");
        splineContainer = splineObj.AddComponent<SplineContainer>();
        splineObj.tag = "Spline";
        return splineObj;
    }

    //<summary> generate a series of knots along the top of a given cube
    //<param : cube> the cube on which to generate the spline 
    public void GenerateKnots(GameObject cube)
    {
        if (cube == null || splineContainer == null)
        {
            Debug.LogError("Cube or SplineContainer is not assigned!");
            return;
        }

        //get bounds of cube and set min and max knot spawning values
        Bounds bounds = cube.GetComponent<Renderer>().bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        //calc z bounds leaving some space on left and right of track
        float minZBound = min.z + spaceValue;
        float maxZBound = max.z - spaceValue;
        //0.01 is added so spline sits on top of cube
        float topY = bounds.max.y + 0.01f;

        //initialize the list
        totalKnots = new List<BezierKnot>();
        Vector3 rightDirection = cube.transform.right;

        //create first knot perpendicular to edge left edge
        float randomZ = Random.Range(minZBound, maxZBound);
        Vector3 startPos = new Vector3(min.x, topY, randomZ);
        totalKnots.Add(new BezierKnot(startPos, -rightDirection, rightDirection, Quaternion.identity));

        //keep track of previous positoin and direction, for angle calculations
        Vector3 prevKnotPos = startPos;
        Vector3 prevDirection = rightDirection;

        //generate rest of knots along a random inc from min.x to max.x
        float xPosition = min.x + UnityEngine.Random.Range(minStepX, maxStepX);

        //create knots until end of cube is reached
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
            totalKnots.Add(new BezierKnot(knotPosition, -prevDirection, prevDirection, quaternion.identity));
            prevKnotPos = knotPosition;

            //calculate next knot position
            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);

            //if position would be more than max.x break
            if (nextX > max.x + 1f)
            {
                break;
            }

            xPosition = nextX;
        }

        //create an open spline with list
        splineContainer.Spline = new Spline(totalKnots, closed: false);

        //important: DONT autosmooth first knot, will mess up tangents
        for (int i = 1; i < splineContainer.Spline.Count; i++)
        {
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }

        //set first and last knot as continuous
        splineContainer.Spline.SetTangentMode(0, TangentMode.Continuous);
        splineContainer.Spline.SetTangentMode(splineContainer.Spline.Count - 1, TangentMode.Continuous);

        //add road script
        splineObj.AddComponent<LoftRoadBehaviour>().IncreaseWidthsCount(0);
    }

    //<summary> gets the WORLD position of the FIRST knot in a spline
    //<param : givenSpline> the spline to check
    //<returns> WORLD position of the first knot
    public Vector3 firstKnotPos(GameObject givenSpline)
    {
        Vector3 pos = givenSpline.GetComponent<SplineContainer>().Spline.EvaluatePosition(0f);
        pos = givenSpline.transform.TransformPoint(pos);
        return pos;
    }

    //<summary> gets the WORLD position of the LAST knot in a spline
    //<param : givenSpline> the spline to check
    //<returns> WORLD position of the last knot
    public Vector3 lastKnotPos(GameObject givenSpline)
    {
        Vector3 pos = givenSpline.GetComponent<SplineContainer>().Spline.EvaluatePosition(1f);
        pos = givenSpline.transform.TransformPoint(pos);
        return pos;
    }

    //<summary> calculates the rotation at the last knot of the spline
    //<summary> adds a -90 y adjust
    //<param : givenSpline> the spline to check
    //<returns> adjusted rotation of the last knot
    public Quaternion lastKnotRot(GameObject givenSpline)
    {
        SplineContainer container = givenSpline.GetComponent<SplineContainer>();

        //get tangent and up vector from last knot
        Vector3 endTangent = container.Spline.EvaluateTangent(1f);
        Vector3 endUpVector = container.Spline.EvaluateUpVector(1f);

        //normalize
        endTangent.Normalize();
        endUpVector.Normalize();

        //scrapped calculations
        //Vector3 rightVector = Vector3.Cross(endTangent, endUpVector).normalized;
        //Vector3 correctedUpVector = Vector3.Cross(rightVector, endTangent).normalized;

        //create rotation from vectors
        Quaternion orientation = Quaternion.LookRotation(endTangent, endUpVector);

        //add correction and return
        Quaternion correction = Quaternion.Euler(0, -90, 0);
        orientation = orientation * correction;
        return orientation;
    }

    //<summary> gets the WORLD tangent of the last knot in a spline
    //<param : givenSpline> the spline to check
    //<returns> WORLD tangent of last knot
    public float3 GetLastKnotTan(GameObject givenSpline)
    {
        var container = givenSpline.GetComponent<SplineContainer>();
        var spline = container.Spline;
        var lastKnot = spline[spline.Count - 1];

        //convert from local to world space
        Vector3 worldTangent = givenSpline.transform.TransformDirection(lastKnot.TangentOut);

        return worldTangent;
    }

    //<summary> modify the first knot in a spline to match a given tangent and rotation
    //<summary> used to connect first and last knot of 2 splines
    //<param : tan> the tangent to apply
    //<param : givenSpline> the spline to modify
    //<param : rotation> the rotation to apply
    public void AlterFirstKnot(float3 tan, GameObject givenSpline, Quaternion rotation)
    {
        var container = givenSpline.GetComponent<SplineContainer>();
        var spline = container.Spline;
        BezierKnot firstKnot = spline[0];

        //convert from worlspace to local space
        Vector3 localTangent = givenSpline.transform.InverseTransformDirection(tan);

        //set both tangents (mirrored)
        firstKnot.TangentOut = localTangent;
        firstKnot.TangentIn = -localTangent;

        //get y rotation
        float yRotation = rotation.eulerAngles.y;
        //convert knot rotation to Euler angles
        float3 knotEuler = math.degrees(math.Euler(firstKnot.Rotation));
        //apply only y rotation
        knotEuler.y += yRotation;
        //convert back to quaternion
        firstKnot.Rotation = quaternion.Euler(math.radians(knotEuler));

        //update the knot
        spline.SetKnot(0, firstKnot);
    }

    //<summary> checks if given position is off the spline
    //<param : _pos> position to checl
    //<param : _minDist> minimum distance to consider off road
    //<returns> true if pos is off road, false if pos is on road
    public bool IsOffRoad(Vector3 _pos, float _minDist)
    {
        foreach (BezierKnot bezierKnot in totalKnots)
        {
            // If in a certian distance to road return false
            if (Vector3.Distance(bezierKnot.Position, _pos) < _minDist)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsAngleBetweenCellsAcceptable(GameObject previousSpline, GameObject currentSpline, float maxAllowedAngle)
    {
        SplineContainer prevContainer = previousSpline.GetComponent<SplineContainer>();
        Vector3 prevTangent = prevContainer.Spline.EvaluateTangent(1f);
        prevTangent = previousSpline.transform.TransformDirection(prevTangent).normalized;

        SplineContainer currContainer = currentSpline.GetComponent<SplineContainer>();
        Vector3 currTangent = currContainer.Spline.EvaluateTangent(0f);
        currTangent = currentSpline.transform.TransformDirection(currTangent).normalized;

        float angle = Vector3.Angle(prevTangent, currTangent);
        Debug.Log($"Connection angle: {angle} degrees");

        return angle <= maxAllowedAngle;
    }
}
