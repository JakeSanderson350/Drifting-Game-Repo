using System.Collections;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private TempCell cell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCar());
    }

    private IEnumerator SpawnCar()
    {
        yield return new WaitForSeconds(0.2f);

        car.transform.position = cell.GetFirstKnotPos() + Vector3.up * 5;
    }
}
