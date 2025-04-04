using System;
using UnityEngine;

public class CarState : MonoBehaviour
{
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private float speedToHealthFactor = 0.5f;
    private int health;

    [SerializeField] private Rigidbody carRB;
    private float carSpeed;

    public static Action onCarDeath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        carSpeed = carRB.linearVelocity.magnitude;

        if (transform.position.y < -12)
        {
            onCarDeath.Invoke();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Obstacle hit");
            health -= (int)(carSpeed * speedToHealthFactor);

            if (health <= 0)
            {
                onCarDeath.Invoke();
            }
        }
    }

    public float GetHealth01()
    {
        return (float)(health) / (float)(startingHealth);
    }
}
