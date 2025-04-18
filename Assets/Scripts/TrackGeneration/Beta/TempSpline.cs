using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class TempSpline : MonoBehaviour
{
    [Header("Spline Mesh Info")]
    public SplineContainer roadContain; //container holds spline data
    public SplineContainer grassContain; //container holds spline data
    public GameObject roadObj;            //object the spline container is attached to
    public GameObject grassObj;            //object the spline container is attached to
    public List<BezierKnot> roadKnots;     //collection of knots in current spline
    public List<BezierKnot> grassKnots;     //collection of knots in current spline
    public Material grassMaterial;
    private float avgSplineLength = 252.0f; //avg length of splines used to calc position of obstacles

    [Space(10)]
    [Header("Step Values")]
    public float minStepX;                  //min distance between knots on x axis
    public float maxStepX;                  //max distance ....
    public float spaceValue;                //buffer space from edge of bounds
    public float maxAngle;                  //max angle allowed between knots (to prevent sharp turns)

    [Space(10)]
    [Header("Bounds Settings")]
    public Vector3 boundsMin = new Vector3(-5, 0, -5);  // Minimum bounds coordinates
    public Vector3 boundsMax = new Vector3(5, 0, 5);    // Maximum bounds coordinates
    public float splineHeight = 0.01f;                  // Height above the bounds where spline will be placed

    [Space(10)]
    [Header("Descent Settings")]
    public bool enableDescent = false;                
    public float minDescentRate;           
    public float maxDescentRate;              
    public float descentVariation;        
    public float maxDescentAngle;          

    const int maxAttempts = 20;         

    //<summary> initalizes a new spline gameobject with components
    //<returns> the newly created spline object
    public GameObject Init()
    {
        roadObj = new GameObject("Road Spline");
        roadContain = roadObj.AddComponent<SplineContainer>();
        roadObj.tag = "Spline";

        grassObj = new GameObject("Grass Spline");
        grassContain = grassObj.AddComponent<SplineContainer>();
        grassObj.tag = "Spline";

        return roadObj;
    }

    //<summary> used to grab a reference to only the grass spline
    //<returns> grass spline gameobject
    public GameObject GetGrassSpline()
    {
        return grassObj;
    }

    //<summary> generate a series of knots within given bounds
    public void GenerateKnotsInBounds()
    {
        if (roadContain == null)
        {
            Debug.LogError("SplineContainer is not assigned!");
            return;
        }

        // Use the defined bounds instead of getting them from a cube
        Vector3 min = boundsMin;
        Vector3 max = boundsMax;

        // Calculate Z bounds leaving some space on left and right of track
        float minZBound = min.z + spaceValue;
        float maxZBound = max.z - spaceValue;

        // Set base height where the spline will start
        float startY = max.y + splineHeight;
        float currentY = startY;

        // Initialize the list
        roadKnots = new List<BezierKnot>();
        Vector3 rightDirection = Vector3.right;

        // Create first knot perpendicular to left edge
        float randomZ = Random.Range(minZBound, maxZBound);
        Vector3 startPos = new Vector3(min.x, startY, randomZ);
        roadKnots.Add(new BezierKnot(startPos, -rightDirection, rightDirection, Quaternion.identity));

        // Keep track of previous position and direction, for angle calculations
        Vector3 prevKnotPos = startPos;
        Vector3 prevDirection = rightDirection;
        float prevY = startY;
        float totalTrackLength = max.x - min.x;
        float currentTrackProgress = 0f;

        // Generate rest of knots along a random increment from min.x to max.x
        float xPosition = min.x + UnityEngine.Random.Range(minStepX, maxStepX);

        // Create knots until end of bounds is reached
        while (xPosition < max.x)
        {
            Vector3 knotPosition = Vector3.zero;

            // Calculate progress along the track
            currentTrackProgress = (xPosition - min.x) / totalTrackLength;

            // Calculate Y position with only downward movement
            if (enableDescent)
            {
                // Calculate base descent amount for this step
                float stepDistance = xPosition - prevKnotPos.x;
                float baseDescentAmount = Random.Range(minDescentRate, maxDescentRate) * stepDistance;

                // Add some variation based on progress (can be more steep towards end)
                float variationFactor = 1f + (descentVariation * currentTrackProgress);
                float descentAmount = baseDescentAmount * variationFactor;

                // Calculate new Y position (only going down, never up)
                currentY = prevY - descentAmount;
            }
            else
            {
                // No descent, stay at the base height
                currentY = startY;
            }

            // Try to get a non harsh angle between knots
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Generate position with calculated height
                randomZ = UnityEngine.Random.Range(minZBound, maxZBound);
                knotPosition = new Vector3(xPosition, currentY, randomZ);

                // Calculate direction in 3D
                Vector3 newDirection = (knotPosition - prevKnotPos).normalized;

                // Calculate horizontal angle (ignore y component for path smoothness)
                Vector3 flatPrevDir = new Vector3(prevDirection.x, 0, prevDirection.z).normalized;
                Vector3 flatNewDir = new Vector3(newDirection.x, 0, newDirection.z).normalized;
                float horizontalAngle = Vector3.Angle(flatPrevDir, flatNewDir);

                // Calculate descent angle to ensure it's not too steep
                float verticalAngle = 0;
                if (enableDescent)
                {
                    float heightDiff = prevY - currentY; // Positive value for descent
                    float horizontalDist = Vector3.Distance(
                        new Vector3(knotPosition.x, 0, knotPosition.z),
                        new Vector3(prevKnotPos.x, 0, prevKnotPos.z));

                    if (horizontalDist > 0.001f) // Prevent division by zero
                    {
                        verticalAngle = Mathf.Atan2(heightDiff, horizontalDist) * Mathf.Rad2Deg;
                    }
                }

                // If both horizontal and vertical angles are acceptable, point is valid
                if (horizontalAngle < maxAngle && verticalAngle < maxDescentAngle)
                {
                    prevDirection = newDirection;
                    break;
                }

                // If we failed to find a valid angle, adjust the descent
                if (attempt == maxAttempts - 1 && enableDescent)
                {
                    // Reduce descent amount to make it easier to find valid angle
                    currentY = Mathf.Lerp(prevY, currentY, 0.5f);
                    knotPosition.y = currentY;
                }
            }

            // Add the knot
            roadKnots.Add(new BezierKnot(knotPosition, -prevDirection, prevDirection, quaternion.identity));
            prevKnotPos = knotPosition;
            prevY = knotPosition.y;

            // Calculate next knot position
            float nextX = xPosition + UnityEngine.Random.Range(minStepX, maxStepX);

            // If position would be more than max.x break
            if (nextX > max.x + 1f)
            {
                break;
            }

            xPosition = nextX;
        }

        // Create an open spline with list
        roadContain.Spline = new Spline(roadKnots, closed: false);
        grassContain.Spline = new Spline(roadKnots, closed: false);

        // Important: DONT autosmooth first knot, will mess up tangents
        for (int i = 1; i < roadContain.Spline.Count; i++)
        {
            roadContain.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
            grassContain.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }

        // Set first and last knot as continuous
        roadContain.Spline.SetTangentMode(0, TangentMode.Continuous);
        roadContain.Spline.SetTangentMode(roadContain.Spline.Count - 1, TangentMode.Continuous);

        grassContain.Spline.SetTangentMode(0, TangentMode.Continuous);
        grassContain.Spline.SetTangentMode(roadContain.Spline.Count - 1, TangentMode.Continuous);

        // Add road script
        roadObj.AddComponent<LoftRoadBehaviour>().IncreaseWidthsCount(0);
        roadObj.AddComponent<MeshCollider>();

        grassObj.AddComponent<LoftRoadBehaviour>().IncreaseWidthsCount(1);
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

        // Get tangent and up vector from last knot
        Vector3 endTangent = container.Spline.EvaluateTangent(1f);
        Vector3 endUpVector = container.Spline.EvaluateUpVector(1f);

        // Normalize
        endTangent.Normalize();
        endUpVector.Normalize();

        // Create rotation from vectors
        Quaternion orientation = Quaternion.LookRotation(endTangent, endUpVector);

        // Add correction and return
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

        // Convert from local to world space
        Vector3 worldTangent = givenSpline.transform.TransformDirection(lastKnot.TangentOut);

        return worldTangent;
    }

    //<summary> modify the first knot in a spline to match a given tangent and rotation
    //<summary> used to connect first and last knot of 2 splines
    //<param : tan> the tangent to apply
    //<param : givenSpline> the spline to modify
    //<param : rotation> the rotation to apply
    public void AlterFirstKnot(float3 tan, GameObject givenSpline, GameObject grassSpline, Quaternion rotation)
    {
        var container = givenSpline.GetComponent<SplineContainer>();
        var spline = container.Spline;
        BezierKnot firstKnot = spline[0];

        // Convert from worlspace to local space
        Vector3 localTangent = givenSpline.transform.InverseTransformDirection(tan);

        // Set both tangents (mirrored)
        firstKnot.TangentOut = localTangent;
        firstKnot.TangentIn = -localTangent;

        // Get y rotation
        float yRotation = rotation.eulerAngles.y;
        // Convert knot rotation to Euler angles
        float3 knotEuler = math.degrees(math.Euler(firstKnot.Rotation));
        // Apply only y rotation
        knotEuler.y += yRotation;
        // Convert back to quaternion
        firstKnot.Rotation = quaternion.Euler(math.radians(knotEuler));

        // Update the knot
        spline.SetKnot(0, firstKnot);

        var gContainer = givenSpline.GetComponent<SplineContainer>();
        var gSpline = gContainer.Spline;
        gSpline.SetKnot(0, firstKnot);
    }

    //<summary> changes the grass spline so it looks different than road
    //<param : > the grass spline to modify
    public void AlterGrassSpline(GameObject gSpline)
    {
        var container = gSpline.GetComponent<SplineContainer>();
        var spline = container.Spline;

        //change material
        gSpline.GetComponent<MeshRenderer>().material = grassMaterial;

        //change shader
        gSpline.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");

        //move down to prevent z fighting
        Vector3 pos = gSpline.transform.position; 
        gSpline.transform.position = new Vector3(pos.x, pos.y - 0.5f, pos.z);
    }

    public Vector3 GetRandomSpawnPos(float _minDist)
    {
        float t = Random.Range(0.0f, 1.0f);
        Vector3 splinePos = roadContain.EvaluatePosition(t);

        Vector2 horizontalOffset = Random.insideUnitCircle * _minDist;
        float y = roadContain.EvaluatePosition(t + (horizontalOffset.y / avgSplineLength)).y;
        Vector3 spawnPos = new Vector3(splinePos.x + horizontalOffset.x, y - 0.75f, splinePos.z + horizontalOffset.y);

        return spawnPos;
    }

    //<summary> checks if given position is off the spline
    //<param : _pos> position to checl
    //<param : _minDist> minimum distance to consider off road
    //<returns> true if pos is off road, false if pos is on road
    public bool IsOffRoad(Vector3 _pos, float _minDist)
    {
        foreach (BezierKnot bezierKnot in roadKnots)
        {
            // If in a certian distance to road return false
            if (Vector3.Distance(bezierKnot.Position, _pos) < _minDist)
            {
                return false;
            }
        }

        return true;
    }
}