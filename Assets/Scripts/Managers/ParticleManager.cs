using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarDriftController carDriftController;
    [SerializeField] private Transform[] trailAttachPoints;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Material trailMaterial;

    [Header("Layer Configuration")]
    [SerializeField] private LayerMask roadLayerMask;

    [Header("Color Configuration")]
    [SerializeField] private Color initialColor = Color.blue;
    [SerializeField] private Color finalColor = Color.red;
    [SerializeField] private float colorTransitionTime = 5f;

    [Header("Drift Effects Configuration")]
    [SerializeField] private float minDriftAngleForEffects = 3f;
    [SerializeField] private float particleEmissionRate = 250f;

    [Header("Trail Configuration")]
    [SerializeField] private float trailWidth = 8f;        // Base trail width
    [SerializeField] private float trailMaxWidth = 14f;    // Max trail width
    [SerializeField] private float trailHeightOffset = 0.01f;
    [SerializeField] private float trailTime = 1.8f;

    [Header("Particle Configuration")]
    [SerializeField] private float particleSpawnRadius = 1.8f;
    [SerializeField] private float particleSpawnHeightOffset = 0.2f;
    [SerializeField] private float particleSpawnDistance = 2.5f;  // Distance behind the trail

    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
    private List<Transform> particleAttachPoints = new List<Transform>();  // Separate attachment points for particles

    private bool isDrifting;
    private float driftTime;
    private Color currentColor;

    private void Start()
    {
        // Ensure we have a default material
        if (trailMaterial == null)
        {
            trailMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
            trailMaterial.color = initialColor;
        }

        InitializeEffectSystems();

        // Make sure all effects are disabled at start
        SetEffectsActive(false);
    }

    private void InitializeEffectSystems()
    {
        // Validate and find references
        carDriftController ??= GetComponentInParent<CarDriftController>();

        if (trailAttachPoints == null || trailAttachPoints.Length == 0)
            FindWheelTransforms();

        if (particlePrefab == null)
        {
            Debug.LogError("No particle prefab assigned!");
            return;
        }

        // Create effects for each attach point
        foreach (Transform attachPoint in trailAttachPoints)
        {
            CreateTrailRenderer(attachPoint);
            CreateParticleAttachPoint(attachPoint);
        }

        // Make sure all effects are initially disabled
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.enabled = false;
            trail.Clear();
        }

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void FindWheelTransforms()
    {
        var wheelTransforms = new List<Transform>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name.Contains("Wheel") || t.name.Contains("Tire"))
                wheelTransforms.Add(t);
        }
        trailAttachPoints = wheelTransforms.ToArray();
    }

    private void CreateParticleAttachPoint(Transform wheelAttachPoint)
    {
        // Create a new attach point for particles that will be positioned behind the wheel
        GameObject particleAttachObj = new GameObject($"ParticleAttach_{wheelAttachPoint.name}");
        particleAttachObj.transform.SetParent(transform, true);

        // Position will be dynamically updated in Update()
        Transform particleAttach = particleAttachObj.transform;
        particleAttachPoints.Add(particleAttach);

        // Create particle system at this new attachment point
        CreateParticleSystem(particleAttach);
    }

    private void CreateParticleSystem(Transform attachPoint)
    {
        GameObject particleObj = Instantiate(particlePrefab, attachPoint.position, Quaternion.identity, attachPoint);
        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>()
            ?? particleObj.AddComponent<ParticleSystem>();

        ConfigureParticleSystem(particleSystem);
        particleSystems.Add(particleSystem);
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }

    private void CreateTrailRenderer(Transform attachPoint)
    {
        GameObject trailObj = new GameObject($"DriftTrail_{attachPoint.name}");
        trailObj.transform.SetParent(attachPoint, false);

        TrailRenderer trailRenderer = trailObj.AddComponent<TrailRenderer>();
        ConfigureTrailRenderer(trailRenderer);
        trailRenderers.Add(trailRenderer);
    }

    private void ConfigureParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = initialColor;
        main.startLifetime = 2.0f;
        main.startSize = 1.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 2.0f; // Fixed initial speed to avoid compatibility issues
        main.playOnAwake = false; // Prevent playing automatically on awake

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.burstCount = 1;
        emission.SetBurst(0, new ParticleSystem.Burst(0.0f, 3, 5, 1, 0.1f));

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = particleSpawnRadius;
        shape.position = new Vector3(0, particleSpawnHeightOffset, 0);
        shape.rotation = new Vector3(-90, 0, 0); // Point cone upward and backward

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;

        // Set all velocity curves in the same mode (MinMax)
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.5f, 1.2f);  // Variable upward velocity
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);

        // Add rotation to particles
        var rotationOverLifetime = ps.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-90f, 90f);

        // Add size over lifetime for particles to shrink
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;

        // Use consistent curve mode
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 1.0f);
        curve.AddKey(1.0f, 0.3f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        sizeOverLifetime.separateAxes = false;

        // Initially stop the particle system
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void ConfigureTrailRenderer(TrailRenderer trail)
    {
        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = trailWidth * 0.7f;  // Even less taper for much wider trails
        trail.minVertexDistance = 0.05f;     // More detail in the trail for smoother curves
        trail.widthMultiplier = 1.2f;        // Additional width multiplier for even larger trails

        trail.material = trailMaterial;
        trail.startColor = initialColor;
        trail.endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f);

        trail.receiveShadows = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.generateLightingData = true;
        trail.autodestruct = false;
    }

    private void AdjustTrailToRoad()
    {
        for (int i = 0; i < trailRenderers.Count; i++)
        {
            TrailRenderer trail = trailRenderers[i];
            Transform attachPoint = trail.transform.parent;
            Vector3 raycastOrigin = attachPoint.position;

            if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 10f, roadLayerMask))
            {
                trail.transform.position = hit.point + (Vector3.up * trailHeightOffset);
                trail.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                // Update particle attach point to be behind the wheel in the driving direction
                if (i < particleAttachPoints.Count)
                {
                    // Get car's velocity direction or use wheel's right vector if velocity not available
                    Vector3 movementDirection = Vector3.zero;
                    Rigidbody carRigidbody = carDriftController.GetComponent<Rigidbody>();

                    if (carRigidbody != null && carRigidbody.linearVelocity.magnitude > 0.1f)
                    {
                        // Use the actual movement direction of the car
                        movementDirection = -carRigidbody.linearVelocity.normalized;
                    }
                    else
                    {
                        // Fallback to wheel orientation if velocity is too low
                        movementDirection = -attachPoint.forward; // Using forward as driving direction
                    }

                    // Calculate position well behind the wheel based on movement direction
                    Vector3 offsetPosition = hit.point + (Vector3.up * (trailHeightOffset + particleSpawnHeightOffset))
                                          + (movementDirection * particleSpawnDistance);
                    particleAttachPoints[i].position = offsetPosition;
                }
            }
            else
            {
                Debug.LogWarning($"No road detected for trail at {raycastOrigin}");
            }
        }
    }

    private void Update()
    {
        if (carDriftController == null) return;

        UpdateDriftState();
        UpdateEffectColors();
        AdjustTrailToRoad();
        UpdateTrailWidth();
    }

    private void UpdateDriftState()
    {
        float driftAngle = Mathf.Abs(carDriftController.GetDriftAngle());
        bool isDriftingNow = driftAngle > minDriftAngleForEffects;

        if (!isDrifting && isDriftingNow)
            StartDrift();
        else if (isDriftingNow)
            ContinueDrift();
        else if (!isDriftingNow)
            EndDrift();
    }

    private void StartDrift()
    {
        isDrifting = true;
        driftTime = 0f;
        currentColor = initialColor;
        SetEffectsActive(true);
    }

    private void ContinueDrift()
    {
        isDrifting = true;
        driftTime += Time.deltaTime;
        currentColor = Color.Lerp(currentColor, GetTargetColor(), Time.deltaTime * 2f);
    }

    private Color GetTargetColor()
    {
        if (driftTime > colorTransitionTime * 2)
            return finalColor;

        if (driftTime > colorTransitionTime)
        {
            float t = (driftTime - colorTransitionTime) / colorTransitionTime;
            return Color.Lerp(Color.yellow, finalColor, t);
        }

        float initialT = driftTime / colorTransitionTime;
        return Color.Lerp(initialColor, Color.yellow, initialT);
    }

    private void EndDrift()
    {
        isDrifting = false;
        driftTime = 0f;
        SetEffectsActive(false);
    }

    private void UpdateTrailWidth()
    {
        if (!isDrifting) return;

        float dynamicWidth = Mathf.Lerp(trailWidth, trailMaxWidth, driftTime / colorTransitionTime);
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.startWidth = dynamicWidth;
            trail.endWidth = dynamicWidth * 0.7f;  // Maintain the 0.7 ratio for less taper

            // Apply a sine wave effect to the trail width for an even more dramatic effect
            float pulseAmount = Mathf.Sin(Time.time * 8f) * 0.15f + 1.0f;
            trail.widthMultiplier = 1.2f * pulseAmount;
        }
    }

    private void UpdateEffectColors()
    {
        if (!isDrifting) return;

        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = currentColor;

            var emission = ps.emission;
            emission.rateOverTime = particleEmissionRate;
        }

        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.startColor = currentColor;
            trail.endColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
        }
    }

    private void SetEffectsActive(bool active)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            var emission = ps.emission;
            emission.rateOverTime = active ? particleEmissionRate : 0;

            if (active)
            {
                if (!ps.isPlaying) ps.Play();
            }
            else
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        foreach (TrailRenderer trail in trailRenderers)
        {
            if (trail.enabled != active)
            {
                trail.enabled = active;

                // Clear the trail when enabling to prevent sudden appearance
                if (active)
                {
                    trail.Clear();
                }
            }
        }
    }

    public void ReconfigureEffectSystems(Transform[] newAttachPoints, GameObject newParticlePrefab = null, Material newTrailMaterial = null)
    {
        // Clean up existing particle systems and trails
        foreach (ParticleSystem ps in particleSystems)
            Destroy(ps.gameObject);
        particleSystems.Clear();

        foreach (TrailRenderer trail in trailRenderers)
            Destroy(trail.gameObject);
        trailRenderers.Clear();

        foreach (Transform attachPoint in particleAttachPoints)
            Destroy(attachPoint.gameObject);
        particleAttachPoints.Clear();

        trailAttachPoints = newAttachPoints;

        if (newParticlePrefab != null)
            particlePrefab = newParticlePrefab;

        if (newTrailMaterial != null)
            trailMaterial = newTrailMaterial;

        InitializeEffectSystems();
    }
}