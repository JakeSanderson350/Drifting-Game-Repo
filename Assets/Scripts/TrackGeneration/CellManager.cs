using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    [Header("Cell Info")]
    private List<Cell> activeCells = new List<Cell>();
    public Cell currentCell;
    public GameObject cellPrefab;
    public float maxLoadedCells = 6;

    public Transform player;

    void Start()
    {
        GenerateInitialCells();
    }
    void Update()
    {
        UpdateCells(player.position);
    }

    //Generates the initial 3-5 loaded cells w track
    private void GenerateInitialCells()
    {
    }
    //Instaniates the next cell prefab
    //calls trackmanager.CarveTrack with the just created cell prefab
    private void GenerateNextCell()
    {
        TrackManager.instance.CarveTrack(currentCell);
    }
    //deals with the creation and deletion of cells
    //based on players current position
    //calls generateNextCell() when necessary
    private void UpdateCells(Vector3 playerPos)
    {
    
    }
}
