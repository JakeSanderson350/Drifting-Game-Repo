using System.Collections;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private DownCell cell;
    [SerializeField] private Cell cellFlat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCar());
    }

    private IEnumerator SpawnCar()
    {
        yield return new WaitForSeconds(0.2f);

        if (cell != null)
        {
            car.transform.position = cell.GetFirstKnotPos() + Vector3.up * 5;
        }
        else if (cellFlat != null)
        {
            car.transform.position = cellFlat.GetFirstKnotPos() + Vector3.up * 5;
        }
        else
        {
            Debug.Log("No spawn point found");
        }
    }
}
