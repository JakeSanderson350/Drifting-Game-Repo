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

    public GameObject Init(Vector3 prevPos)
    {
        Debug.Log("Cube Init");

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh = cube.GetComponent<MeshFilter>().mesh;
        cube.GetComponent<MeshRenderer>().material = tempMaterial;

        cube.transform.localScale = new Vector3(lengthX, heightY, widthZ);

        //spawn first cube at 0,0,0
        if(prevPos == Vector3.zero)
        {
            cube.transform.position = new Vector3(prevPos.x, prevPos.y, prevPos.z);
        }

        cube.transform.position = new Vector3(prevPos.x + (lengthX / 2), 0, prevPos.z);
        //new Vector3(prevPos.x + (lengthX / 2), prevPos.y - (heightY / 2), prevPos.z);

        return cube;
    }
}
