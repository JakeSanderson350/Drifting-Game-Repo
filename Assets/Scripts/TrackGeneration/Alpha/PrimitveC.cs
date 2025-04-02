using Unity.VisualScripting;
using UnityEngine;

public class PrimitveC : MonoBehaviour
{
    [Header("Mesh Values")]
    public GameObject cube;
    public Mesh mesh;
    public Material tempMaterial;

    [Space(10)]
    [Header("Cube Dimension Values")]
    public float lengthX;
    public float heightY;
    public float widthZ;

    public GameObject Init()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh = cube.GetComponent<MeshFilter>().mesh;
        cube.GetComponent<MeshRenderer>().material = tempMaterial;

        cube.transform.localScale = new Vector3(lengthX, heightY, widthZ);

        cube.transform.position = Vector3.zero;
        cube.transform.eulerAngles = Vector3.zero;
        return cube;
    }

    public void alterRotation(Quaternion prevRot, Vector3 lastKnotPos, Vector3 firstKnotPos)
    {

        float distance = Vector3.Distance(lastKnotPos, firstKnotPos);
        Vector3 directionVector = lastKnotPos - firstKnotPos;
        Vector3 normalizedDirection = directionVector.normalized;

        Debug.Log("Distance between points: " + distance);
        Debug.Log("Direction vector: " + normalizedDirection.ToString("F8"));

        cube.transform.rotation = prevRot;
    }
}
