using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("References")]
    public SplineC splineGen;
    public PrimitveC cubeGen;
    public GameObject cube;

    [Header("Timer Values")]
    public float timerDuration;
    private float currentTimerTime;

    [Header("Previous Values")]
    public Vector3 lastKnotPos;
    public Quaternion lastKnotRot;

    public void Start()
    {
        lastKnotPos = Vector3.zero;
        currentTimerTime = timerDuration;
    }

    public void Update()
    {
        UpdateTimer();
    }

    public void GenerateCell()
    {
        //generate cube with respect to last knot position
        cube = cubeGen.Init(lastKnotPos);

        splineGen.Init();
        splineGen.AttachCube(cube);

        lastKnotPos = splineGen.lastKnotPos();
        lastKnotRot = splineGen.lastKnotRot();
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
