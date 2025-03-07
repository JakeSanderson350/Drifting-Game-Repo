using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.ExtrusionShapes;

public class GDCGeneration : MonoBehaviour
{
    public Vector2[] verts;
    public Vector2[] normals;
    public float[] uv;
    public int[] lines = new int[] 
    {
        0, 1,
        2, 3,
        3, 4,
        4, 5
    };

    void Start()
    {
        //creating mesh
        MeshFilter mf = new MeshFilter();
        Mesh mesh = mf.sharedMesh;

        Vector3[] vertices =
        {
            new Vector3 (1f, 0f, 1f),
            new Vector3 (-1f, 0f, 1f),
            new Vector3 (1f, 0f, -1f),
            new Vector3 (-1f, 0f, -1f),
        };

        Vector3[] normals =
        {
            new Vector3 (0f, 1f, 0f),   //aka Vector3.up
            new Vector3 (0f, 1f, 0f),
            new Vector3 (0f, 1f, 0f),
            new Vector3 (0f, 1f, 0f),
        };

        Vector2[] uvs =
        {
            new Vector2 (0, 1),
            new Vector2 (0, 0),
            new Vector2 (1, 1),
            new Vector2 (1, 0),
        };

        int[] triangles = new int[]
        {
            0, 2, 3,    //1st 
            3, 1, 0     //2nd
        };

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    //using bernstein polynomials (optimized)
    Vector3 GetPoint(Vector3[] points, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        return points[0] * (omt2 * omt) +
               points[1] * (3f * omt2 * t) +
               points[2] * (3f * omt * t2) +
               points[3] * (t2 * t);
    }
    Vector3 GetTangent(Vector3[] points, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 tangent =
            points[0] * (-omt2) +
            points[1] * (3 * omt2 - 2 * omt) +
            points[2] * (-3 * t2 + 2 * t) +
            points[3] * (t2);

        return tangent.normalized;
    }
    Vector3 GetNormal3D(Vector3[] points, float t, Vector3 up)
    { 
        Vector3 tangent = GetTangent(points, t);
        Vector3 biNormal = Vector3.Cross(up, tangent).normalized;

        return Vector3.Cross(tangent, biNormal);
    }

    Quaternion GetOrientation3D(Vector3[] points, float t, Vector3 up)
    {
        Vector3 tangent = GetTangent(points, t);
        Vector3 normal = GetNormal3D(points, t, up);

        return Quaternion.LookRotation(tangent, normal);
    }

    public void Extrude(Mesh mesh, IExtrudeShape shape, OrientedPoint[] path)
    {
        List<float2> vertices = new List<float2>();
        shape = new Road(); // Or Square(), Road(), etc.
        for (int i = 0; i < shape.SideCount; i++)
        {
            vertices.Add(shape.GetPosition(0f, i)); // t is usually not needed
        }

        int vertsInShape = 1;// shape.vertices.Length;
        int segments = path.Length - 1;
        int edgeLoops = path.Length;

        int vertCount = vertsInShape * edgeLoops;
        int triCount = 1;//shape.lines.Length * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndicies = new int[triIndexCount];
        Vector3[] vertices2     = new Vector3[vertCount];
        Vector3[] normals      = new Vector3[vertCount];
        Vector2[] uvs          = new Vector2[vertCount];

        /*Mesh Generation Code Here*/
        for(int i = 0; i < path.Length; i++)
        {
            int offset = i * vertsInShape;

            for(int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;
               // vertices[id]    = path[i].LocalToWorld(shape.vert2Ds[j].point);
               // normals[id]     = path[i].LocalToWorldDirection(shape.vert2Ds[j].normal);
               // uvs[id]         = new Vector2(verts2Ds[j].uCoord, i / ((float)edgeLoops) );
            }
        }

        int ti = 0;
        for(int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for(int l = 0; l < lines.Length; l += 2)
            {
                int a = offset + lines[l] + vertsInShape;
                int b = offset + lines[l];
                int c = offset + lines[l +1];
                int d = offset + lines[l +1] + vertsInShape;

                triangleIndicies[ti] = a;   ti++;
                triangleIndicies[ti] = b;   ti++;
                triangleIndicies[ti] = c;   ti++;
                triangleIndicies[ti] = c;   ti++;
                triangleIndicies[ti] = d;   ti++;
                triangleIndicies[ti] = a;   ti++;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices2;
        mesh.triangles = triangleIndicies;
        mesh.normals = normals;
        mesh.uv = uvs;
    }

    public struct OrientedPoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public OrientedPoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        public Vector3 LocalToWorld(Vector3 point)
        {
            return position + rotation * point;
        }
        public Vector3 WorldToLocal(Vector3 point)
        {
            return Quaternion.Inverse(rotation) * (point - position);
        }
        public Vector3 LocalToWorldDirection(Vector3 dir)
        {
            return rotation * dir;
        }
    }
}
