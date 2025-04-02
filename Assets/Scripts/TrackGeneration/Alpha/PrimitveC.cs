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
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh = cube.GetComponent<MeshFilter>().mesh;
        cube.GetComponent<MeshRenderer>().material = tempMaterial;

        cube.transform.localScale = new Vector3(lengthX, heightY, widthZ);

        if (prevPos == Vector3.zero)
        {   //spawn first cube at 0,0,0
            cube.transform.position = Vector3.zero;
        }
        else
        {
            cube.transform.position = new Vector3(prevPos.x + (lengthX / 2), 0, prevPos.z);
        }
        return cube;
    }

    public void alterRotation(float prevRot)
    {
        Debug.Log("cube alteration: " + prevRot);
        cube.transform.rotation = Quaternion.Euler(0, prevRot, 0);
    }
}
