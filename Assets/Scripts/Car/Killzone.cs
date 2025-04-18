using UnityEngine;
using System;

public class Killzone : MonoBehaviour
{
    public static Action onCarDeath;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onCarDeath.Invoke();
        }
    }
}
