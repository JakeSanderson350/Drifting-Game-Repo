using System;
using UnityEngine;

public class CarState : MonoBehaviour
{
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private float speedToHealthFactor = 0.5f;
    private int health;

    [SerializeField] private Rigidbody carRB;
    private float carSpeed;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private AudioClip carDeathSound;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    public static Action onCarDeath;
    public static Action togglePause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePause.Invoke();
        }

        carSpeed = carRB.linearVelocity.magnitude;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Obstacle hit");

            health -= (int)(carSpeed * speedToHealthFactor);

            if (health <= 0)
            {
                PlayCarDeathSound();
                PlayScream();
                onCarDeath.Invoke();
            }
            else
            {
                PlayCollisionSound();
            }
        }
    }

    public float GetHealth01()
    {
        return (float)(health) / (float)(startingHealth);
    }

    public void PlayCollisionSound()
    {
        float speedRatio = Mathf.Clamp01(carSpeed / 30f);

        audioSource.volume = Mathf.Lerp(minVolume, maxVolume, speedRatio);
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);

        audioSource.PlayOneShot(collisionSound);
    }

    private void PlayCarDeathSound()
    {
        audioSource.volume = 0.8f;
        audioSource.pitch = 1.0f;

        audioSource.PlayOneShot(carDeathSound);
    }
    private void PlayScream()
    {
        audioSource.volume = maxVolume;
        audioSource.pitch = 1.0f;

        audioSource.PlayOneShot(screamSound);
    }
}
