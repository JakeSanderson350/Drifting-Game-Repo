using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Track Information")]
    public Vector3 trackStartPos;
    public Vector3 trackEndPos;
    public Quaternion trackEndRotation;

    [Header("Cell Information")]
    public Bounds cellBounds;
    public Vector3 cellPosition;
    public Terrain cellTerrain;
    //public float[,] heightMap;

    public Cell()
    {
    }
}
