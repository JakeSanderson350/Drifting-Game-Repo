using UnityEngine;

public class CollisionDebrisManager : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem debrisParticlePrefab;
    [SerializeField] private int minParticleCount = 15;   // Increased default minimum
    [SerializeField] private int maxParticleCount = 50;   // Increased default maximum
    [SerializeField] private float minParticleSpeed = 3f; // Slightly increased
    [SerializeField] private float maxParticleSpeed = 10f;// Slightly increased
    [SerializeField] private float particleLifetime = 3f;

    [Header("Collision Settings")]
    [SerializeField] private float speedToParticlesMultiplier = 5f;  // Increased from 2f to 5f

    [Header("References")]
    [SerializeField] private Rigidbody vehicleRigidbody;

    private void Start()
    {
        // If vehicle rigidbody not assigned, try to get it from this GameObject
        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = GetComponent<Rigidbody>();
        }

        // Ensure we have a debris particle prefab
        if (debrisParticlePrefab == null)
        {
            Debug.LogError("Debris Particle Prefab not assigned to CollisionDebrisManager!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Obstacle hit - spawning debris particles");

            // Get vehicle speed
            float vehicleSpeed = vehicleRigidbody.linearVelocity.magnitude;

            // Calculate number of particles based on speed
            int particleCount = Mathf.Clamp(
                Mathf.RoundToInt(vehicleSpeed * speedToParticlesMultiplier),
                minParticleCount,
                maxParticleCount
            );

            Debug.Log($"Vehicle speed: {vehicleSpeed}, Spawning {particleCount} particles in total");

            // Spawn particles at each contact point
            foreach (ContactPoint contact in collision.contacts)
            {
                // Get exact collision point and normal
                Vector3 collisionPoint = contact.point;
                Vector3 collisionNormal = contact.normal;

                // Spawn particle effect at collision point
                ParticleSystem spawnedParticleSystem = Instantiate(
                    debrisParticlePrefab,
                    collisionPoint,
                    Quaternion.LookRotation(collisionNormal)
                );

                // Configure the particle system
                var mainModule = spawnedParticleSystem.main;
                mainModule.startLifetime = particleLifetime;

                // Don't divide by contact count - just use the full particle count for each contact point
                // This ensures more particles regardless of how many contact points exist

                var emissionModule = spawnedParticleSystem.emission;
                emissionModule.SetBurst(0, new ParticleSystem.Burst(0f, particleCount));

                // Make sure rate over time is zero (we only want the initial burst)
                emissionModule.rateOverTime = 0;

                // Set particle direction to burst outward from collision point
                var shapeModule = spawnedParticleSystem.shape;
                shapeModule.shapeType = ParticleSystemShapeType.Cone;
                shapeModule.angle = 35f; // Wider angle for more spread

                // Set particle speed based on vehicle speed
                var velocityModule = spawnedParticleSystem.velocityOverLifetime;
                float particleSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, vehicleSpeed / 30f);
                velocityModule.speedModifier = particleSpeed;

                // Play the particle effect
                spawnedParticleSystem.Play();

                // Destroy after lifetime
                Destroy(spawnedParticleSystem.gameObject, particleLifetime + 0.5f);

                // Log for debugging
                Debug.Log($"Spawned particle system at point {collisionPoint} with {particleCount} particles");
            }
        }
    }
}