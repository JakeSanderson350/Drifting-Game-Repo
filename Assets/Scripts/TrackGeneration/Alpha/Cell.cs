using UnityEngine;
using UnityEngine.Splines;

public class Cell : MonoBehaviour
{
    [Header("References")]
    public SplineC splineGen;
    public PrimitveC cubeGen;
    public GameObject cube;
    public GameObject spline;
    public GameObject empty;

    [Header("Timer Values")]
    public float timerDuration;
    private float currentTimerTime;

    [Header("Previous Values")]
    public Vector3 lastKnotPos;
    public Vector3 firstKnotPos;
    public Quaternion lastKnotRot;

    private int tempIndex;

    public void Start()
    {
        lastKnotPos = Vector3.zero;
        lastKnotRot = Quaternion.identity;
        currentTimerTime = timerDuration;
        tempIndex = 0;
    }

    public void Update()
    {
        UpdateTimer();
    }

    public void GenerateCell()
    {
        //init cube and spline, generate points on spline
        cube = cubeGen.Init();
        spline = splineGen.Init();
        splineGen.GenerateKnots(cube, lastKnotRot);

        //change the rotation of both cube and spline
        alterRotation(lastKnotRot);

        //get first point in spline in worldspace
        firstKnotPos = splineGen.firstKnotPos(spline);

        //alter position so THIS first knot matches with LAST last knot
        alterPosition(lastKnotPos, firstKnotPos);

        //get last knot (pos/rot) of THIS spline
        lastKnotPos = splineGen.lastKnotPos(spline);
        lastKnotRot = splineGen.lastKnotRot(spline);

        tempIndex++;
    }
    public void alterRotation(Quaternion prevRot)
    {
        cube.transform.rotation = prevRot;
        spline.transform.rotation = prevRot;
    }
    public void alterPosition(Vector3 lastKnotPos, Vector3 firstKnotPos)
    {
        if (tempIndex == 0) { return; }

        float distance = Vector3.Distance(lastKnotPos, firstKnotPos);
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 newPositionCube = cube.transform.position + normalizedDirection * distance;
        cube.transform.position = new Vector3(newPositionCube.x, 0f, newPositionCube.z);

        Vector3 newPositionSpline = spline.transform.position + normalizedDirection * distance;
        spline.transform.position = new Vector3(newPositionSpline.x, 0f, newPositionSpline.z);

        //Debug.Log("Distance between points: " + distance);
        //Debug.Log("Direction vector: " + normalizedDirection.ToString("F8"));
    }
    private void UpdateTimer()
    {
        currentTimerTime -= Time.deltaTime;

        if (currentTimerTime <= 0.0)
        {
            ResetTimer();
            GenerateCell();
        }
    }

    private void ResetTimer()
    {
        currentTimerTime = timerDuration;
    }
}
