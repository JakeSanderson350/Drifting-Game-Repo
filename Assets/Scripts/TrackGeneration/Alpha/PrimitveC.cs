using UnityEngine;

public class PrimitveC : MonoBehaviour
{
    [Header("Mesh Object")]
    public GameObject cube;
    public Mesh mesh;
    public Material temp;

    [Header("Scaling Values")]
    public float lengthX;
    public float heightY;
    public float widthZ;

    public GameObject Init(Vector3 prevPos)
    {
        Debug.Log("Cube Init");

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh = cube.GetComponent<MeshFilter>().mesh;
        cube.GetComponent<MeshRenderer>().material = temp;

        cube.transform.localScale = new Vector3(lengthX, heightY, widthZ);

        cube.transform.position = new Vector3(prevPos.x + (lengthX / 2), prevPos.y - (heightY / 2), prevPos.z);

        return cube;
    }
}
