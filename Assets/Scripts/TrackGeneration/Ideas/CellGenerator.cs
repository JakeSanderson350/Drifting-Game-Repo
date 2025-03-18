using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGenerator : MonoBehaviour
{
    [Header("Mesh Values")]
    public Vector3[] vertices = new Vector3[]
    {
        //bottom face 
        new Vector3(4, -1, 10), new Vector3(-4, -1, 10),
        new Vector3(4, -1, -1), new Vector3(-4, -1, -1),
        //top face 
        new Vector3(4, 1, 10), new Vector3(-4, 1, 10),
        new Vector3(4, 1, -1), new Vector3(-4, 1, -1), 

        //front face -> doesnt do anything?
        new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),  
        //back face -> doesnt do anything?
        new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), 
        //left face -> doesnt do anything?
        new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f), 
        //right face -> doesnt do anything?
        new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f),
    };
    public Vector3[] normals = new Vector3[]
    {
        //bottom face 
        new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
        new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
        //top face 
        new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
        new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
        //front face 
        new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
        new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
        //back face 
        new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
        new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
        //left face
        new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
        new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
        //right face
        new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
        new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
    };
    public Vector2[] uvs0 = new Vector2[]       //UV 0
    {
        //bottom face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //top face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //front face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //back face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //left face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //right face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
    };
    public Vector2[] uvs1 = new Vector2[]       //UV 1
    {
        //bottom face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //top face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //front face
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //back face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //left face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        //right face 
        new Vector2(0f, 0f), new Vector2(1f, 0f),
        new Vector2(0f, 1f), new Vector2(1f, 1f),
    };
    public int[] triangles = new int[]
    {
        //bottom face
        0, 2, 3,
        3, 1, 0,
        //top face
        4, 5, 7,
        7, 6, 4,
        //front face
        0, 1, 5,
        5, 4, 0,
        //back face
        2, 6, 7,
        7, 3, 2,
        //left face
        1, 3, 7,
        7, 5, 1,
        //right face
        0, 4, 6,
        6, 2, 0,
    };
    public Vector4[] tangents = new Vector4[]
    {
        //bottom face 
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        //top face
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        //front face
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        //back face 
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        //left face 
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        //right face 
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
    };

    [Header("Mesh Object")]
    public GameObject cube;
    public Mesh mesh;
    public Material temp;
    [Tooltip("Event that is sent after Cell is complete")]
    public bool finished = false;

    void Start()
    {
        GenerateWithValues();
    }
    public void PrimitiveGeneration()
    {
        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

    }
/*    public void OnValidate()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.tangents = tangents;
        mesh.uv = uvs0;
        mesh.uv2 = uvs1;
    }*/
    public void GenerateWithValues()
    {
        mesh = new Mesh();
        mesh.name = "Custom Mesh";

        cube = new GameObject("Procedural Cube");
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();
        cube.GetComponent<MeshFilter>().mesh = mesh;
        cube.GetComponent<MeshRenderer>().material = temp;
        cube.AddComponent<BoxCollider>();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.tangents = tangents;
        mesh.uv = uvs0;
        mesh.uv2 = uvs1;

        GameObject tempObj = GameObject.FindGameObjectWithTag("SplineGen");
        tempObj.GetComponent<SplineGenerator>().SetCell(cube);
    }
}
