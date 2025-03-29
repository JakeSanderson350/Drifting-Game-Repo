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
        cube = cubeGen.Init(lastKnotPos);
        splineGen.Init();
        lastKnotPos = splineGen.AttachCube(cube);
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
