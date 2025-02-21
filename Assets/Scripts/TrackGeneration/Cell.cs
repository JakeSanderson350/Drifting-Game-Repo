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

    void Start()
    {
        init();
    }

    public void init()
    {
        GameObject cube = new GameObject("Procedural Cube");
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        cube.GetComponent<MeshFilter>().mesh = mesh;

        // vertices
        Vector3[] vertices = 
            {
            new Vector3(-25, -25, -10), new Vector3(25, -25, -10), new Vector3(25, 25, -10), new Vector3(-25, 25, -10), 
            new Vector3(-25, -25, 10), new Vector3(25, -25, 10), new Vector3(25, 25, 10), new Vector3(-25, 25, 10)  
        };

        // triangles 
        int[] triangles = 
        {
            0, 2, 1,  0, 3, 2,  // Back
            4, 5, 6,  4, 6, 7,  // Front
            0, 1, 5,  0, 5, 4,  // Bottom
            2, 3, 7,  2, 7, 6,  // Top
            0, 4, 7,  0, 7, 3,  // Left
            1, 2, 6,  1, 6, 5   // Right
        };

        // normals 
        Vector3[] normals = 
         {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back, // Back
            Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward // Front
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        // material
        cube.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
    }
}
