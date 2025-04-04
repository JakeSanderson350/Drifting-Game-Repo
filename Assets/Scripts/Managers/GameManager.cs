using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void OnEnable()
    {
        CarState.onCarDeath += GameOver;
    }

    private void OnDisable()
    {
        CarState.onCarDeath -= GameOver;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GameOver()
    {

    }
}
