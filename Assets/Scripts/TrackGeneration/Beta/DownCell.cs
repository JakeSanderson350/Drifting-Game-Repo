using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Hierarchy;
using Unity.Mathematics;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class DownCell : MonoBehaviour
{
    [Header("References")]
    public DownSpline splineGen;
    private GameObject grassSpline;
    [SerializeField] private GameObject killzonePrefab;

    [Header("Previous Values")]
    private Vector3 lastKnotPos;
    private Vector3 firstKnotPos;
    [SerializeField] private Quaternion lastKnotRot;
    private Unity.Mathematics.float3 lastKnotTangent;

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private int numObstaclesPerCell = 10;
    [SerializeField] private float obstaclesDistToRoad = 4.0f;

    [Header("Trigger Settings")]
    public Vector3 triggerSize = new Vector3(0.1f, 2f, 15f);  // Size of the trigger collider
    public Vector3 boundsMin = new Vector3(-5, 0, -5);  // Minimum bounds coordinates
    public Vector3 boundsMax = new Vector3(5, 0, 5);    // Maximum bounds coordinates

    [Header("Connection Angle")]
    public bool useConnectionAngleCheck = false;
    public float maxAngle = 90f;
    public float maxAttmepts = 10;

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
        GameObject roadSpline = splineGen.Init();
        splineGen.GenerateKnotsInBounds();
        grassSpline = splineGen.GetGrassSpline();

        //add underneath the empty gameobject
        roadSpline.transform.SetParent(cellObject.transform);
        grassSpline.transform.SetParent(cellObject.transform);

        //create and set up trigger
        GameObject newTrigger = CreateTrigger();
        newTrigger.transform.SetParent(cellObject.transform);

        //add obstacles
        List<GameObject> cellObstacles = CreateObstacles(roadSpline);

        //scale transforms
        ScaleTransforms(roadSpline, grassSpline, newTrigger);

        //apply rotation based on previous cell
        AlterRotation(lastKnotRot, roadSpline, grassSpline, newTrigger); 
        //get first point on spline
        firstKnotPos = splineGen.firstKnotPos(roadSpline);

        //DO NOT alter position on first cell
        if (firstCellRendered)
        {
            if (useConnectionAngleCheck)
            {
                CheckConnectionAngle(roadSpline, cellObject, newTrigger);
            }

            //alter position to connect with previous cell
            AlterPosition(lastKnotPos, firstKnotPos, roadSpline, grassSpline);
            splineGen.AlterFirstKnot(lastKnotTangent, roadSpline, grassSpline, lastKnotRot);
        }

        //store last knot info
        lastKnotPos = splineGen.lastKnotPos(roadSpline);
        lastKnotRot = splineGen.lastKnotRot(roadSpline);
        lastKnotTangent = splineGen.GetLastKnotTan(roadSpline);

        //move trigger to the end of this spline
        newTrigger.transform.position = lastKnotPos;

        //create killzone underneath cell
        GameObject killzone = CreateKillzone(roadSpline);
        killzone.transform.SetParent(cellObject.transform);

        //alter grass spline
        splineGen.AlterGrassSpline(grassSpline);

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
    private GameObject CreateTrigger()
    {
        GameObject trigger = new GameObject("EdgeTrigger");

        // Position trigger at the end of the bounds
        Vector3 triggerPosition = new Vector3(
            boundsMax.x,                         // Place at maximum X (end of track)
            (boundsMin.y + boundsMax.y) * 0.5f,  // Center on Y axis
            (boundsMin.z + boundsMax.z) * 0.5f   // Center on Z axis
        );

        trigger.transform.position = triggerPosition;

        //add box collider
        BoxCollider triggerCollider = trigger.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.transform.localScale = triggerSize;

        //add a trigger script to listen for for player
        TriggerHandlerTemp triggerScript = trigger.AddComponent<TriggerHandlerTemp>();
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
        int scaledNumObstacles = numObstaclesPerCell + GameManagerED.Instance.GetDifficulty();

        for (int i = 0; i < scaledNumObstacles; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool isValidSpawnPos = false;

            do
            {
                //spawnPos = new Vector3(Random.Range(-xRange, xRange), 0.0f, Random.Range(-zRange, zRange));
                spawnPos = splineGen.GetRandomSpawnPos(obstaclesDistToRoad);
                isValidSpawnPos = splineGen.IsOffRoad(spawnPos, obstaclesDistToRoad);
            } while (!isValidSpawnPos);

            GameObject newObstacle = Instantiate(obstaclePrefabs[UnityEngine.Random.Range(0, obstaclePrefabs.Count)], spawnPos, parentCube.transform.rotation);
            newObstacles.Add(newObstacle);
            newObstacle.transform.SetParent(parentCube.transform, true); // bc parented, obstacles will move when cube is moved
        }

        return newObstacles;
    }

    private GameObject CreateKillzone(GameObject parentObj)
    {
        return Instantiate(killzonePrefab, parentObj.transform.position + Vector3.down * 70, parentObj.transform.rotation);
    }

    //<summary> scales cube and spline by scale factor
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    private void ScaleTransforms(GameObject spline, GameObject grassSpline, GameObject trigger)
    {
            //cube.transform.localScale *= scaleFactor;
        spline.transform.localScale *= scaleFactor;
        grassSpline.transform.localScale *= scaleFactor;
        trigger.transform.localScale *= scaleFactor;

            //cube.layer = 6;
        spline.layer = 6;
        grassSpline.layer = 6;
    }

    //<summary> alters rotation of the spline, cube, the triggerand obstacles
    //<summary> to match the rotation of the previous last knot
    //<param : prevRot> the rotation of the last knot in the previous spline
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    //<param : obstacles> obstacles in cell
    public void AlterRotation(Quaternion prevRot, GameObject spline, GameObject grassSpline, GameObject trigger) 
    {
        spline.transform.rotation = prevRot;
        grassSpline.transform.rotation = prevRot;
        trigger.transform.rotation = prevRot * spline.transform.rotation;
    }

    //<summary> alters position of the spline and cube 
    //<summary> to match the current first knot, to the previous last knot
    //<param : lastKnotPos> the position of the last knot in the previous cell
    //<param : firstKnotPos> the position of the first knot in the current cell
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    public void AlterPosition(Vector3 lastKnotPos, Vector3 firstKnotPos, GameObject spline, GameObject grassSpline)
    {
        float distance = Vector3.Distance(lastKnotPos, firstKnotPos) - 0.5f;
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 newPosSpline = spline.transform.position + normalizedDirection * distance;
        spline.transform.position = new Vector3(newPosSpline.x, newPosSpline.y, newPosSpline.z);

        Vector3 newGrassPos = grassSpline.transform.position + normalizedDirection * distance;
        grassSpline.transform.position = new Vector3(newGrassPos.x, newGrassPos.y, newGrassPos.z);
    }

    public Vector3 GetFirstKnotPos()
    {
        return (Vector3)activeCellObjects[0].transform.Find("Road Spline")?.GetComponent<SplineContainer>()?.EvaluatePosition(0.1f);
    }

    // <summary> checks if the angle between the last knot of the previous cell and first knot of current cell is acceptable
    // <param : prevLastKnotTangent> Tangent of the last knot in previous cell
    // <param : newFirstKnotPos> position of the first knot in new cell
    // <param : prevLastKnotPos> position of the last knot in previous cell
    // <returns> true if angle is below 90 degrees, false otherwise
    private bool IsConnectionAngleAcceptable(Vector3 prevLastKnotTangent, Vector3 newFirstKnotPos, Vector3 prevLastKnotPos)
    {
        //direction vector
        Vector3 connectionDirection = newFirstKnotPos - prevLastKnotPos;
        connectionDirection.Normalize();

        //normalize
        Vector3 normalizedTangent = prevLastKnotTangent.normalized;

        //calc angle
        float angle = Vector3.Angle(normalizedTangent, connectionDirection);

        Debug.Log($"Connection angle: {angle} degrees");

        //return true if angle is below angle
        return angle < maxAngle;
    }

    public void CheckConnectionAngle(GameObject roadSpline, GameObject cellObject, GameObject newTrigger)
    {
        int attempt = 0;

        // Get first knot position of current cell and check angle
        Vector3 newFirstKnotPos = splineGen.firstKnotPos(roadSpline);

        // While angle is not acceptable and we haven't exceeded max attempts
        while (!IsConnectionAngleAcceptable(lastKnotTangent, newFirstKnotPos, lastKnotPos) && attempt < maxAttmepts)
        {
            // Destroy current spline and grass spline
            Destroy(roadSpline);
            Destroy(grassSpline);

            // Regenerate spline
            roadSpline = splineGen.Init();
            splineGen.GenerateKnotsInBounds();
            grassSpline = splineGen.GetGrassSpline();

            // Re-parent and re-apply transforms
            roadSpline.transform.SetParent(cellObject.transform);
            grassSpline.transform.SetParent(cellObject.transform);

            // Scale and rotate again
            ScaleTransforms(roadSpline, grassSpline, newTrigger);
            AlterRotation(lastKnotRot, roadSpline, grassSpline, newTrigger);

            // Get updated first knot position
            newFirstKnotPos = splineGen.firstKnotPos(roadSpline);

            attempt++;
        }
    }
}

//<summary> handles trigger events for individual cells, notifying the cell manager
public class TriggerHandlerTemp : MonoBehaviour
{
    public DownCell cellManager;
    public int cellIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //tell the cell manager that this trigger was activated
            cellManager.OnCellTriggered(cellIndex);

            //get rid of the trigger after so players cant spam its
            Destroy(this.gameObject);
        }
    }
}