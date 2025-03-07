using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [Header("Cell Info")]
    public List<PrefabCell> prefabs = new List<PrefabCell>();
    public List<PrefabCell> activeCells = new List<PrefabCell>();
    public PrefabCell currentCell;
    public PrefabCell previousCell;
    private int currentCellIndex;
    public float maxLoadedCells = 10;

    void Start()
    {
        currentCellIndex = (int)Random.Range(0f, prefabs.Count);
        currentCell = prefabs[currentCellIndex];

        FirstCell(currentCell);
        StartCoroutine(Wait());
    }
    private void FirstCell(PrefabCell cell)
    {
        PrefabCell newCell = Instantiate(cell, new Vector3(0, 0, 0), Quaternion.identity);
        activeCells.Add(newCell);
        previousCell = newCell;
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3.0f);
        GenerateCells();
    }
    private void GenerateCells()
    {
        currentCellIndex = (int)Random.Range(0f, prefabs.Count);
        currentCell = prefabs[currentCellIndex];

        Vector3 spawnPosition = previousCell.trackEndPos;

        //add offset so cells are not overlapping
        float offsetZ = previousCell.bounds.size.z / 2f;
        spawnPosition += new Vector3(0, 0, offsetZ);

        //instantiate cell and spline values
        currentCell = Instantiate(currentCell, spawnPosition, previousCell.trackEndRotation);
        currentCell.setTrackPos();

        //adding more offset after spline is created to match up the tracks
        float offsetX = (currentCell.trackStartPos.x - previousCell.trackEndPos.x);
        offsetZ = (currentCell.trackStartPos.z - previousCell.trackEndPos.z);
        currentCell.transform.position -= new Vector3(offsetX,0,offsetZ);

        previousCell = currentCell;
        StartCoroutine(Wait());
    }
}
