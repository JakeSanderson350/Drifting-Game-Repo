using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Cell Information")]
    //Values that work, but create a square with missing reflections and shadings
    public Vector3[] vertices = new Vector3[]
    {
            // Bottom face
            new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f),
            new Vector3(1f, -1f, -1f), new Vector3(-1f, -1f, -1f),
            // Top face
            new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f),
            new Vector3(1f, 1f, -1f), new Vector3(-1f, 1f, -1f),
    };
    public Vector3[] normals = new Vector3[]
    {
            // Bottom Face
            new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
            new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
            // Top Face
            new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
            new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
            // Front Face
            new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
            // Back Face
            new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
            // Left Face
            new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
            //Right Face
            new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
    };
    public Vector2[] uvs = new Vector2[]
    {
            // Bottom face
            new Vector2(0, 1), new Vector2(0, 0),
            new Vector2(1, 1), new Vector2(1, 0),
            // Top face
            new Vector2(0, 1), new Vector2(0, 0),
            new Vector2(1, 1), new Vector2(1, 0),
    };

    //values that make the created square hold the same values/memory space as one generated through regular means
    /*    public Vector3[] vertices = new Vector3[]
        {
            // Bottom face 
            new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f), 
            new Vector3(1f, -1f, -1f), new Vector3(-1f, -1f, -1f),
            // Top face 
            new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f), 
            new Vector3(1f, 1f, -1f), new Vector3(-1f, 1f, -1f), 
            // Front face 
            new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f), 
            new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f),  
            // Back face 
            new Vector3(1f, -1f, -1f), new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 1f, -1f), new Vector3(-1f, 1f, -1f), 
            // Left face
            new Vector3(-1f, -1f, 1f), new Vector3(-1f, -1f, -1f),
            new Vector3(-1f, 1f, 1f), new Vector3(-1f, 1f, -1f), 
            // Right face 
            new Vector3(1f, -1f, 1f), new Vector3(1f, -1f, -1f), 
            new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, -1f), 
        };
        public Vector3[] normals = new Vector3[]
        {
            // Bottom face 
            new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
            new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f),
            // Top face 
            new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
            new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),
            // Front face 
            new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f),
            // Back face 
            new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f),
            // Left face
            new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
            // Right face
            new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f),
        };
        public Vector4[] tangents = new Vector4[]
        {
            // Bottom face 
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            // Top face
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            // Front face
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            // Back face 
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            // Left face 
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            // Right face 
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 0f, 0f, 1f),
        };
        public Vector2[] uvs0 = new Vector2[]
        {
            // Bottom face UV0 
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Top face UV0 
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Front face UV0
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Back face UV0
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Left face UV0
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Right face UV0
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
        };
        public Vector2[] uvs1 = new Vector2[]
        {
            // Bottom face UV1
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Top face UV1
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Front face UV1
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Back face UV1 
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Left face UV1
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            // Right face UV1 
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
        };*/

    public int[] triangles = new int[]
    {
        // Bottom face
        0, 2, 3,
        3, 1, 0,
        // Top face
        4, 5, 7,
        7, 6, 4,
        // Front face
        0, 1, 5,
        5, 4, 0,
        // Back face
        2, 6, 7,
        7, 3, 2,
        // Left face
        1, 3, 7,
        7, 5, 1,
        // Right face
        0, 4, 6,
        6, 2, 0,
    };

    private GameObject cube;
    private Mesh mesh;

    void Start()
    {
        cube = new GameObject("Procedural Cube");
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        cube.GetComponent<MeshFilter>().mesh = mesh;
        cube.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        UpdateCell();
    }
    private void OnValidate()
    {
        UpdateCell();
    }

    public void UpdateCell()
    {
/*        // vertices
        vertices = new Vector3[]
        {
            new Vector3 (1f, 0f, 1f),
            new Vector3 (-1f, 0f, 1f),
            new Vector3 (1f, 0f, -1f),
            new Vector3 (-1f, 0f, -1f),
        };

        // triangles 
        triangles = new int[]
        {
            0, 2, 3,    //1st 
            3, 1, 0     //2nd
        };

        // uvs
        uvs = new Vector2[]
        {
            new Vector2 (0, 1),
            new Vector2 (0, 0),
            new Vector2 (1, 1),
            new Vector2 (1, 0),
        };

        // normals 
        normals = new Vector3[]
        {
            new Vector3 (0f, 1f, 0f), 
            new Vector3 (0f, 1f, 0f),
            new Vector3 (0f, 1f, 0f),
            new Vector3 (0f, 1f, 0f),
        };*/

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        //mesh.tangents = tangents;
        //mesh.uv = uvs1;
        //mesh.uv2 = uvs2;
        mesh.triangles = triangles;
    }
}
