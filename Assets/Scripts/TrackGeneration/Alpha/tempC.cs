using System.Collections.Generic;
using NUnit.Framework;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class tempC : MonoBehaviour
{
    [Header("References")]
    public tempS splineGen;
    public PrimitveC cubeGen;
    public GameObject cube;
    public GameObject spline;
    public GameObject empty;

    [Header("Timer Values")]
    public float timerDuration;
    private float currentTimerTime;

    [Header("Previous Values")]
    private Vector3 lastKnotPos;
    private Vector3 firstKnotPos;
    private Quaternion lastKnotRot;
    private Unity.Mathematics.float3 lastKnotTangent;

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obstaclePrefabs;
    private List<GameObject> obstacles;
    [SerializeField] private int numObstaclesPerCell = 10;
    [SerializeField] private float obstaclesDistToRoad = 4.0f;

    private float scaleFactor = 5.0f;
    private int tempIndex;

    public void Start()
    {
        lastKnotPos = Vector3.zero;
        lastKnotRot = Quaternion.identity;
        currentTimerTime = 0f;
        obstacles = new List<GameObject>();
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
        splineGen.GenerateKnots(cube);

        //add obstacles to cell
        InitObstacles();

        //scale transforms
        ScaleTransforms();

        //change the rotation of both cube and spline based on rotation of the last knot in last cell
        AlterRotation(lastKnotRot);

        //get first point in current spline in worldspace
        firstKnotPos = splineGen.firstKnotPos(spline);

        if (tempIndex > 0)
        {
            //alter position so THIS first knot matches with LAST last knot
            AlterPosition(lastKnotPos, firstKnotPos);
            splineGen.AlterFirstKnot(lastKnotTangent, spline, lastKnotRot);
        }
        //get last knot (pos/rot) of THIS spline
        lastKnotPos = splineGen.lastKnotPos(spline);
        lastKnotRot = splineGen.lastKnotRot(spline);
        lastKnotTangent = splineGen.GetLastKnotTan(spline);

        obstacles.Clear();
        tempIndex++;
    }
    
    //alters rotation of the spline, cube, and obstacles
    //to match the rotation of the previous last knot
    public void AlterRotation(Quaternion prevRot)
    {
        cube.transform.rotation = prevRot;
        spline.transform.rotation = prevRot;

        foreach (GameObject obstacle in obstacles)
        {
            obstacle.transform.rotation = prevRot;
        }
    }

    //alters position of the spline and cube
    //to match the current first knot, to the previous last knot
    public void AlterPosition(Vector3 lastKnotPos, Vector3 firstKnotPos)
    {
        float distance = Vector3.Distance(lastKnotPos, firstKnotPos) - 0.5f;
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 newPositionCube = cube.transform.position + normalizedDirection * distance;
        cube.transform.position = new Vector3(newPositionCube.x, 0f, newPositionCube.z);

        Vector3 newPositionSpline = spline.transform.position + normalizedDirection * distance;
        spline.transform.position = new Vector3(newPositionSpline.x, 0f, newPositionSpline.z);
    }

    //scales cube and spline by scale factor
    private void ScaleTransforms()
    {
        cube.transform.localScale *= scaleFactor;
        spline.transform.localScale *= scaleFactor;
        cube.layer = 6;
        spline.layer = 6;
    }

    //needs to be called after cube and spline are initted
    private void InitObstacles() 
    {
        float xRange = cubeGen.lengthX / 2;
        float zRange = cubeGen.widthZ / 2;
        int scaledNumObstacles = numObstaclesPerCell + GameManager.Instance.GetDifficulty();

        for (int i = 0; i < scaledNumObstacles; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool isValidSpawnPos = false;

            do
            {
                spawnPos = new Vector3(Random.Range(-xRange, xRange), 0.0f, Random.Range(-zRange, zRange));
                isValidSpawnPos = splineGen.IsOffRoad(spawnPos, obstaclesDistToRoad);
            } while (!isValidSpawnPos);


            GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], spawnPos, cube.transform.rotation);
            obstacles.Add(newObstacle);
            newObstacle.transform.SetParent(cube.transform, true); // bc parented, obstacles will move when cube is moved
        }
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
