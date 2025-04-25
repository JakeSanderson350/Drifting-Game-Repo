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
    [SerializeField] private AudioClip honkSound;
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

                PlayCollisionSound();
        }
    }

    public float GetHealth01()
    {
        return (float)(health) / (float)(startingHealth);
    }

    public void PlayCollisionSound()
    {
        float speedRatio = Mathf.Clamp01(carSpeed / 30f);

        float globalVolume = AudioListener.volume;

        float localVolume = Mathf.Lerp(minVolume, maxVolume, speedRatio) * globalVolume;

        audioSource.volume = localVolume;

        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);

        audioSource.PlayOneShot(collisionSound);
    }

    private void PlayCarDeathSound()
    {
        float globalVolume = AudioListener.volume;
        audioSource.volume = 0.8f * globalVolume;
        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(carDeathSound);
        Debug.Log("Playing death sound");
    }

    private void PlayScream()
    {
        float globalVolume = AudioListener.volume;
        audioSource.volume = maxVolume * globalVolume;
        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(screamSound);
    }
}
