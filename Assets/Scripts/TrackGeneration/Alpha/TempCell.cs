using System.Collections.Generic;
using NUnit.Framework;
using Unity.Hierarchy;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class TempCell : MonoBehaviour
{
    [Header("References")]
    public TempSpline splineGen;

    [Header("Previous Values")]
    private Vector3 lastKnotPos;
    private Vector3 firstKnotPos;
    private Quaternion lastKnotRot;
    private Unity.Mathematics.float3 lastKnotTangent;

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private int numObstaclesPerCell = 10;
    [SerializeField] private float obstaclesDistToRoad = 4.0f;

    [Header("Trigger Settings")]
    public Vector3 triggerSize = new Vector3(0.2f, 2f, 10f);  // Size of the trigger collider
    public Vector3 boundsMin = new Vector3(-5, 0, -5);  // Minimum bounds coordinates
    public Vector3 boundsMax = new Vector3(5, 0, 5);    // Maximum bounds coordinates

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
        GameObject newSpline = splineGen.Init();
        splineGen.GenerateKnotsInBounds();

        //add underneath the empty gameobject
        newSpline.transform.SetParent(cellObject.transform);

        //create and set up trigger
        GameObject newTrigger = CreateTrigger();
        newTrigger.transform.SetParent(cellObject.transform);

        //scale transforms
        ScaleTransforms(newSpline, newTrigger);

        //apply rotation based on previous cell
        AlterRotation(lastKnotRot, newSpline, newTrigger); 
        //get first point on spline
        firstKnotPos = splineGen.firstKnotPos(newSpline);

        //DO NOT alter position on first cell
        if (firstCellRendered)
        {
            //alter position to connect with previous cell
            AlterPosition(lastKnotPos, firstKnotPos, newSpline);
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

    //<summary> scales cube and spline by scale factor
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    private void ScaleTransforms(GameObject spline, GameObject trigger)
    {
            //cube.transform.localScale *= scaleFactor;
        spline.transform.localScale *= scaleFactor;
        trigger.transform.localScale *= scaleFactor;
            //cube.layer = 6;
        spline.layer = 6;
    }

    //<summary> alters rotation of the spline, cube, the triggerand obstacles
    //<summary> to match the rotation of the previous last knot
    //<param : prevRot> the rotation of the last knot in the previous spline
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    //<param : trigger> trigger in cell
    //<param : obstacles> obstacles in cell
    public void AlterRotation(Quaternion prevRot, GameObject spline, GameObject trigger) 
    {
            //cube.transform.rotation = prevRot;
        spline.transform.rotation = prevRot;
        trigger.transform.rotation = prevRot;

        float yRotation = prevRot.eulerAngles.y;
        // Convert knot rotation to Euler angles
        //float3 knotEuler = math.degrees(math.Euler(firstKnot.Rotation));
        // Apply only y rotation
        //knotEuler.y += yRotation;
        // Convert back to quaternion
        //firstKnot.Rotation = quaternion.Euler(math.radians(knotEuler));

        //foreach (GameObject obstacle in obstacles)
        //{
        //    obstacle.transform.rotation = prevRot;
        //}
    }

    //<summary> alters position of the spline and cube 
    //<summary> to match the current first knot, to the previous last knot
    //<param : lastKnotPos> the position of the last knot in the previous cell
    //<param : firstKnotPos> the position of the first knot in the current cell
    //<param : cube> cube in cell
    //<param : spline> spline in cell
    public void AlterPosition(Vector3 lastKnotPos, Vector3 firstKnotPos,GameObject spline)
    {
        float distance = Vector3.Distance(lastKnotPos, firstKnotPos) - 0.5f;
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

            //Vector3 newPosCube = cube.transform.position + normalizedDirection * distance;
            //cube.transform.position = new Vector3(newPosCube.x, newPosCube.y, newPosCube.z);

        Vector3 newPosSpline = spline.transform.position + normalizedDirection * distance;
        spline.transform.position = new Vector3(newPosSpline.x, newPosSpline.y, newPosSpline.z);
    }
}

//<summary> handles trigger events for individual cells, notifying the cell manager
public class TriggerHandlerTemp : MonoBehaviour
{
    public TempCell cellManager;
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