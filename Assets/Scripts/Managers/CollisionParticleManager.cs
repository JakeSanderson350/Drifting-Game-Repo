using UnityEngine;
public class CollisionDebrisManager : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem debrisParticlePrefab;
    [SerializeField] private int minParticleCount = 100;
    [SerializeField] private int maxParticleCount = 300;
    [SerializeField] private float minParticleSpeed = 5f;
    [SerializeField] private float maxParticleSpeed = 15f;
    [SerializeField] private float particleLifetime = 3f;
    [SerializeField] private float spreadAngle = 90f;

    [Header("Collision Settings")]
    [SerializeField] private float speedToParticlesMultiplier = 12f;

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
        if (collision.gameObject.CompareTag("Obstacle"))
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

            Debug.Log($"Vehicle speed: {vehicleSpeed}, Spawning {particleCount} particles");

            // Use the first contact point as the center of the collision
            if (collision.contactCount > 0)
            {
                // Get collision information from the first contact point
                Vector3 collisionPoint = collision.contacts[0].point;
                Vector3 collisionNormal = collision.contacts[0].normal;

                // Spawn particle effect at collision point
                ParticleSystem spawnedParticleSystem = Instantiate(
                    debrisParticlePrefab,
                    collisionPoint,
                    Quaternion.LookRotation(collisionNormal)
                );

                // Configure the particle system
                var mainModule = spawnedParticleSystem.main;
                mainModule.startLifetime = particleLifetime;

                // Add randomness to particle sizes for more dynamic effect
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.5f);

                // Setup for multiple bursts for a more chaotic effect
                var emissionModule = spawnedParticleSystem.emission;
                emissionModule.SetBurst(0, new ParticleSystem.Burst(0f, (int)(particleCount * 0.7f)));
                emissionModule.SetBurst(1, new ParticleSystem.Burst(0.1f, (int)(particleCount * 0.3f)));
                emissionModule.rateOverTime = 0;

                // Wider spread for a crash effect
                var shapeModule = spawnedParticleSystem.shape;
                shapeModule.shapeType = ParticleSystemShapeType.Cone;
                shapeModule.angle = spreadAngle;

                // Add some radius to the emission cone for even wider distribution
                shapeModule.radius = 0.5f;

                // Higher particle speed based on vehicle speed
                var velocityModule = spawnedParticleSystem.velocityOverLifetime;
                float particleSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, vehicleSpeed / 30f);
                velocityModule.speedModifier = particleSpeed;

                // Add some randomness to particle directions
                var forceModule = spawnedParticleSystem.forceOverLifetime;
                forceModule.enabled = true;
                forceModule.x = new ParticleSystem.MinMaxCurve(-2f, 2f);
                forceModule.y = new ParticleSystem.MinMaxCurve(-2f, 2f);
                forceModule.z = new ParticleSystem.MinMaxCurve(-2f, 2f);

                // Play the particle effect
                spawnedParticleSystem.Play();

                // Destroy after lifetime
                Destroy(spawnedParticleSystem.gameObject, particleLifetime + 0.5f);

                Debug.Log($"Spawned single particle system at point {collisionPoint} with {particleCount} particles");
            }
        }
    }
}