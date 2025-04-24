    using UnityEngine;
    public class CollisionDebrisManager : MonoBehaviour
    {
        [Header("Particle Settings")]
        [SerializeField] private ParticleSystem debrisParticlePrefab;
        [SerializeField] private int minParticleCount = 50;
        [SerializeField] private int maxParticleCount = 150;
        [SerializeField] private float minParticleSpeed = 5f;
        [SerializeField] private float maxParticleSpeed = 15f;
        [SerializeField] private float particleLifetime = 3f;

        [Header("Collision Settings")]
        [SerializeField] private float speedToParticlesMultiplier = 8f;  

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

                    // Setup for a single large burst
                    var emissionModule = spawnedParticleSystem.emission;
                    emissionModule.SetBurst(0, new ParticleSystem.Burst(0f, particleCount));
                    emissionModule.rateOverTime = 0;  // Ensure continuous emission is off

                    // Wider spread for a crash effect
                    var shapeModule = spawnedParticleSystem.shape;
                    shapeModule.shapeType = ParticleSystemShapeType.Cone;
                    shapeModule.angle = 60f;

                    // Higher particle speed based on vehicle speed
                    var velocityModule = spawnedParticleSystem.velocityOverLifetime;
                    float particleSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, vehicleSpeed / 30f);
                    velocityModule.speedModifier = particleSpeed;

                    // Play the particle effect
                    spawnedParticleSystem.Play();

                    // Destroy after lifetime
                    Destroy(spawnedParticleSystem.gameObject, particleLifetime + 0.5f);

                    Debug.Log($"Spawned single particle system at point {collisionPoint} with {particleCount} particles");
                }
            }
        }
    }