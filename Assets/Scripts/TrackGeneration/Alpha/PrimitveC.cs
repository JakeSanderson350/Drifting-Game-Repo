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
}
