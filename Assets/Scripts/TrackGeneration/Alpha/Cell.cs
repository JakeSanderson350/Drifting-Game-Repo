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

    public void Start()
    {
        lastKnotPos = Vector3.zero;
        lastKnotRot = Quaternion.identity;
        currentTimerTime = timerDuration;
    }

    public void Update()
    {
        UpdateTimer();
    }

    public void GenerateCell()
    {
        empty = new GameObject("empty");
        cube = cubeGen.Init();
        cube.transform.SetParent(empty.transform, worldPositionStays: true);

        spline = splineGen.Init();
        splineGen.GenerateKnots(cube);
        spline.transform.SetParent(empty.transform, worldPositionStays: true);

        //cubeGen.alterRotation(lastKnotRot);
        alterRotation(lastKnotRot);

        firstKnotPos = splineGen.firstKnotPos(spline);
        cubeGen.alterPosition(lastKnotPos, firstKnotPos);

        lastKnotPos = splineGen.lastKnotPos(spline);
        lastKnotRot = splineGen.lastKnotRot();

    }
    public void alterRotation(Quaternion prevRot)
    {
        Debug.Log("Rotation altered");
        empty.transform.rotation = prevRot;
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
