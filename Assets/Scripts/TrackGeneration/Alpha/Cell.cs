using System.Collections.Generic;
using NUnit.Framework;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Cell : MonoBehaviour
{
    [Header("References")]
    public SplineC splineGen;
    public PrimitveC cubeGen;

    [Header("Previous Values")]
    private Vector3 lastKnotPos;
    private Vector3 firstKnotPos;
    private Quaternion lastKnotRot;
    private Unity.Mathematics.float3 lastKnotTangent;

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private int numObstaclesPerCell = 10;
    [SerializeField] private float obstaclesDistToRoad = 4.0f;

    //cell management
    [SerializeField] private List<GameObject> activeCellObjects = new List<GameObject>();
    private int cellCounter = 0;
    private const int MAX_ACTIVE_CELLS = 3;
    private bool isInitialized = false;
    private bool firstCellRendered;
    private float scaleFactor = 5.0f;

    public void Start()
    {
        lastKnotPos = Vector3.zero;
        lastKnotRot = Quaternion.identity;
        firstCellRendered = false;

        //create first 2 cells
        GenerateInitialCells();
    }

    //<summary> generates first two cells in the track, sets initialized to true
    private void GenerateInitialCells()
    {
        GenerateCell();     //cell 1
        GenerateCell();     //cell 2
        isInitialized = true;
    }

    //<summary> generates a cell and aligns it correctly
    public void GenerateCell()
    {
        cellCounter++;

        //create an empty gameobject
        GameObject cellObject = new GameObject("Cell_" + cellCounter);

        //init cube and spline, generate points on spline
        GameObject newCube = cubeGen.Init();
        GameObject newSpline = splineGen.Init();
        splineGen.GenerateKnots(newCube);

        //add underneath the empty gameobject
        newCube.transform.SetParent(cellObject.transform);
        newSpline.transform.SetParent(cellObject.transform);

        //create and set up trigger
        GameObject newTrigger = CreateTrigger(newCube);
        newTrigger.transform.SetParent(cellObject.transform);

        //add obstacles
        List<GameObject> cellObstacles = CreateObstacles(newCube);

        //scale transforms
        ScaleTransforms(newCube, newSpline, newTrigger);

        //apply rotation based on previous cell
        AlterRotation(lastKnotRot, newCube, newSpline, newTrigger, cellObstacles);

        //get first point on spline
        firstKnotPos = splineGen.firstKnotPos(newSpline);

        //DO NOT alter position on first cell
        if (firstCellRendered)
        {
            //alter position to connect with previous cell
            AlterPosition(lastKnotPos, firstKnotPos, newCube, newSpline);
            splineGen.AlterFirstKnot(lastKnotTangent, newSpline, lastKnotRot);
        }

        //store last knot info
        lastKnotPos = splineGen.lastKnotPos(newSpline);
        lastKnotRot = splineGen.lastKnotRot(newSpline);
        lastKnotTangent = splineGen.GetLastKnotTan(newSpline);

        //move trigger to the end of this spline
        newTrigger.transform.position = lastKnotPos;

        //add to list
        activeCellObjects.Add(cellObject);
        firstCellRendered = true;

        //if we have more than MAX_ACTIVE_CELLS remove oldest
        if (isInitialized && activeCellObjects.Count > MAX_ACTIVE_CELLS)
        {
            RemoveOldestCell();
        }
    }

    //<summary> removes oldest cell from active cells + destroy
    private void RemoveOldestCell()
    {
        if (activeCellObjects.Count > 0)
        {
            GameObject oldestCell = activeCellObjects[0];
            activeCellObjects.RemoveAt(0);
            Destroy(oldestCell);
        }
    }

    //<summary> called when player passes through cell trigger - creates new cell
    //<param : triggeredCellIndex> the index of the cell whose trigger was activated
    public void OnCellTriggered(int triggeredCellIndex)
    {
        //create a new cell when player runs through trigger
        GenerateCell();
    }

    //<summary> creates a trigger object at the edge of a cell to detect when the player crosses it
    //<param : parentCube> cube in cell to act as parent 
    //<returns> fully configured trigger GameObject with collider and handler
    private GameObject CreateTrigger(GameObject parentCube)
    {
        GameObject trigger = new GameObject("EdgeTrigger");

        Bounds cubeBounds = parentCube.GetComponent<Renderer>().bounds;

        //position trigger at the end of the cell
        Vector3 triggerPosition = new Vector3
        (
            cubeBounds.extents.x * scaleFactor,
            cubeBounds.extents.y * scaleFactor,  
            cubeBounds.center.z                
        );
        trigger.transform.position = triggerPosition;

        //add box collider
        BoxCollider triggerCollider = trigger.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;

        //set size of the trigger
        float triggerThickness = 0.2f;
        trigger.transform.localScale = new Vector3
        (
            triggerThickness,
            cubeBounds.size.y * 2,
            cubeBounds.size.z
        );

        //add a trigger script to listen for for player
        TriggerHandler triggerScript = trigger.AddComponent<TriggerHandler>();
        triggerScript.cellManager = this;
        triggerScript.cellIndex = cellCounter;

        return trigger;
    }

    //<summary> spawn obstacles within the cell
    //<summary> make sure obstacles are NOT on road
    //<param : parentCube> cube in cell to act as parent 
    //<returns> list of obstacles in the cell
    private List<GameObject> CreateObstacles(GameObject parentCube)
    {
        List<GameObject> newObstacles = new List<GameObject>();
        float xRange = cubeGen.lengthX / 2;
        float zRange = cubeGen.widthZ / 2;
        int scaledNumObstacles = numObstaclesPerCell + GameManagerED.Instance.GetDifficulty();

        for (int i = 0; i < scaledNumObstacles; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool isValidSpawnPos = false;

            do
            {
                spawnPos = new Vector3(Random.Range(-xRange, xRange), 0.0f, Random.Range(-zRange, zRange));
                isValidSpawnPos = splineGen.IsOffRoad(spawnPos, obstaclesDistToRoad);
            } while (!isValidSpawnPos);

            GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], spawnPos, parentCube.transform.rotation);
            newObstacles.Add(newObstacle);
            newObstacle.transform.SetParent(parentCube.transform, true); // bc parented, obstacles will move when cube is moved
        }

        return newObstacles;
    }

    //<summary> scales cube and spline by scale factor
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    private void ScaleTransforms(GameObject cube, GameObject spline, GameObject trigger)
    {
        cube.transform.localScale *= scaleFactor;
        spline.transform.localScale *= scaleFactor;
        trigger.transform.localScale *= scaleFactor;
        cube.layer = 6;
        spline.layer = 6;
    }

    //<summary> alters rotation of the spline, cube, the triggerand obstacles
    //<summary> to match the rotation of the previous last knot
    //<param : prevRot> the rotation of the last knot in the previous spline
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    //<param : obstacles> obstacles in cell
    public void AlterRotation(Quaternion prevRot, GameObject cube, GameObject spline, GameObject trigger, List<GameObject> obstacles)
    {
        cube.transform.rotation = prevRot;
        spline.transform.rotation = prevRot;
        trigger.transform.rotation = prevRot;

        foreach (GameObject obstacle in obstacles)
        {
            obstacle.transform.rotation = prevRot;
        }
    }

    //<summary> alters position of the spline and cube 
    //<summary> to match the current first knot, to the previous last knot
    //<param : lastKnotPos> the position of the last knot in the previous cell
    //<param : firstKnotPos> the position of the first knot in the current cell
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    public void AlterPosition(Vector3 lastKnotPos, Vector3 firstKnotPos, GameObject cube, GameObject spline)
    {
        float distance = Vector3.Distance(lastKnotPos, firstKnotPos) - 0.5f;
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 newPosCube = cube.transform.position + normalizedDirection * distance;
        cube.transform.position = new Vector3(newPosCube.x, 0f, newPosCube.z);

        Vector3 newPosSpline = spline.transform.position + normalizedDirection * distance;
        spline.transform.position = new Vector3(newPosSpline.x, 0f, newPosSpline.z);
    }
}

//<summary> handles trigger events for individual cells, notifying the cell manager
public class TriggerHandler : MonoBehaviour
{
    public Cell cellManager;
    public int cellIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //tell the cell manager that this trigger was activated
            cellManager.OnCellTriggered(cellIndex);
        }
    }
}