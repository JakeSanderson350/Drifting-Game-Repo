using UnityEngine;

public class SplineBased : MonoBehaviour
{
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
        Vector3 biNormal = Mathf.Cross(up, tangent).normalized;

        return Mathf.Cross(tangent, biNormal);
    }

    Quaternion GetOrientation3D(Vector3[] points, float t, Vector3 up)
    {
        Vector3 tangent = GetTangent(points, t);
        Vector3 normal = GetNormal3D(points, t, up);

        return Quaternion.LookRotation(tangent, normal);
    }
}
