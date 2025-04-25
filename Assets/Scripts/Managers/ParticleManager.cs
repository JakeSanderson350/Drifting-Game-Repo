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
    [SerializeField] private float particleSpawnDistance = 5.0f;

    [Header("Fade Configuration")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
    private List<Transform> particleAttachPoints = new List<Transform>();

    private bool isDrifting;
    private bool isFadingIn;
    private bool isFadingOut;
    private float driftTime;
    private float fadeInTime;
    private float fadeOutTime;
    private Color currentColor;

    // Store original effect values for fade-out
    private List<float> originalEmissionRates = new List<float>();
    private List<float> originalStartWidths = new List<float>();
    private List<float> originalEndWidths = new List<float>();
    private List<Color> originalStartColors = new List<Color>();
    private List<Color> originalEndColors = new List<Color>();

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
        SetEffectsInactive();
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

        // Store original emission rate for fading
        var emission = particleSystem.emission;
        originalEmissionRates.Add(particleEmissionRate);
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

        // Store original values for fading
        originalStartWidths.Add(trailWidth);
        originalEndWidths.Add(trailWidth * 0.7f);
        originalStartColors.Add(initialColor);
        originalEndColors.Add(new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f));
    }

    private void ConfigureParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = initialColor;
        main.startLifetime = 2.0f;
        main.startSize = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 2.0f;
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
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.5f, 1.2f);
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
        trail.endWidth = trailWidth * 0.7f;
        trail.minVertexDistance = 0.05f;
        trail.widthMultiplier = 1.2f;

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
                //Debug.LogWarning($"No road detected for trail at {raycastOrigin}");
            }
        }
    }

    private void Update()
    {
        if (carDriftController == null) return;

        UpdateDriftState();
        AdjustTrailToRoad();

        // Update effects based on current state
        if (isDrifting)
        {
            UpdateEffectColors();
            UpdateTrailWidth();
        }
        else if (isFadingIn)
        {
            UpdateFadeIn();
        }
        else if (isFadingOut)
        {
            UpdateFadeOut();
        }
    }

    private void UpdateDriftState()
    {
        float driftAngle = Mathf.Abs(carDriftController.GetDriftAngle());
        bool isDriftingNow = driftAngle > minDriftAngleForEffects;

        if (!isDrifting && !isFadingIn && !isFadingOut && isDriftingNow)
        {
            StartFadeIn();
        }
        else if ((isFadingIn || isDrifting) && !isDriftingNow)
        {
            StartFadeOut();
        }
        else if (isDrifting)
        {
            ContinueDrift();
        }
    }

    private void StartFadeIn()
    {
        isDrifting = false;
        isFadingIn = true;
        isFadingOut = false;
        fadeInTime = 0f;
        driftTime = 0f;
        currentColor = initialColor;

        EnableEffects();
    }

    private void UpdateFadeIn()
    {
        fadeInTime += Time.deltaTime;
        float progress = Mathf.Clamp01(fadeInTime / fadeInDuration);

        // Use smooth easing for the fade-in
        float easedProgress = progress * progress;

        // Update trail renderers
        foreach (TrailRenderer trail in trailRenderers)
        {
            // Fade in alpha
            Color targetStartColor = initialColor;
            Color targetEndColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f);

            trail.startColor = new Color(
                targetStartColor.r,
                targetStartColor.g,
                targetStartColor.b,
                targetStartColor.a * easedProgress
            );

            trail.endColor = new Color(
                targetEndColor.r,
                targetEndColor.g,
                targetEndColor.b,
                targetEndColor.a * easedProgress
            );

            // Grow width from 0
            trail.startWidth = trailWidth * easedProgress;
            trail.endWidth = (trailWidth * 0.7f) * easedProgress;
        }

        // Update particle systems
        foreach (ParticleSystem ps in particleSystems)
        {
            var emission = ps.emission;
            emission.rateOverTime = particleEmissionRate * easedProgress;

            var main = ps.main;
            Color particleColor = initialColor;
            main.startColor = new Color(
                particleColor.r,
                particleColor.g,
                particleColor.b,
                particleColor.a * easedProgress
            );
        }

        // Transition to full drift when fade-in completes
        if (progress >= 1.0f)
        {
            isDrifting = true;
            isFadingIn = false;
            driftTime = 0f;
        }
    }

    private void StartDrift()
    {
        isDrifting = true;
        isFadingIn = false;
        isFadingOut = false;
        driftTime = 0f;
        currentColor = initialColor;
        EnableEffects();
    }

    private void ContinueDrift()
    {
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

    private void StartFadeOut()
    {
        isDrifting = false;
        isFadingIn = false;
        isFadingOut = true;
        fadeOutTime = 0f;

        // Store current values to fade from
        for (int i = 0; i < trailRenderers.Count; i++)
        {
            if (i < originalStartWidths.Count)
            {
                TrailRenderer trail = trailRenderers[i];
                originalStartWidths[i] = trail.startWidth;
                originalEndWidths[i] = trail.endWidth;
                originalStartColors[i] = trail.startColor;
                originalEndColors[i] = trail.endColor;
            }
        }

        for (int i = 0; i < particleSystems.Count; i++)
        {
            if (i < originalEmissionRates.Count)
            {
                var emission = particleSystems[i].emission;
                originalEmissionRates[i] = emission.rateOverTime.constant;
            }
        }
    }

    private void UpdateFadeOut()
    {
        fadeOutTime += Time.deltaTime;
        float progress = Mathf.Clamp01(fadeOutTime / fadeOutDuration);

        // Use smooth easing for the fade-out
        float easedProgress = 1 - ((1 - progress) * (1 - progress)); // Inverse quadratic easing

        // Update trail renderers
        for (int i = 0; i < trailRenderers.Count; i++)
        {
            if (i >= originalStartWidths.Count) continue;

            TrailRenderer trail = trailRenderers[i];

            // Gradually reduce the trail time for quicker disappearance
            trail.time = Mathf.Lerp(trailTime, 0.1f, easedProgress);

            // Fade out alpha
            Color startColor = originalStartColors[i];
            Color endColor = originalEndColors[i];

            trail.startColor = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                startColor.a * (1.0f - easedProgress)
            );

            trail.endColor = new Color(
                endColor.r,
                endColor.g,
                endColor.b,
                endColor.a * (1.0f - easedProgress)
            );

            // Shrink width
            trail.startWidth = originalStartWidths[i] * (1.0f - easedProgress);
            trail.endWidth = originalEndWidths[i] * (1.0f - easedProgress);
        }

        // Update particle systems
        for (int i = 0; i < particleSystems.Count; i++)
        {
            if (i >= originalEmissionRates.Count) continue;

            ParticleSystem ps = particleSystems[i];
            var emission = ps.emission;
            var main = ps.main;

            // Reduce emission rate
            emission.rateOverTime = originalEmissionRates[i] * (1.0f - easedProgress);

            // Fade particle alpha
            Color particleColor = main.startColor.color;
            main.startColor = new Color(
                particleColor.r,
                particleColor.g,
                particleColor.b,
                particleColor.a * (1.0f - easedProgress)
            );

            // Reduce particle lifetime for quicker fading
            main.startLifetime = Mathf.Lerp(2.0f, 0.5f, easedProgress);
        }

        // Complete fade-out and disable effects
        if (progress >= 1.0f)
        {
            isFadingOut = false;
            SetEffectsInactive();
            ResetEffectsProperties();
        }
    }

    private void ResetEffectsProperties()
    {
        // Reset all trails to their default values
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.time = trailTime;
            trail.startWidth = trailWidth;
            trail.endWidth = trailWidth * 0.7f;
            trail.startColor = initialColor;
            trail.endColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f);
        }

        // Reset all particles to their default values
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = initialColor;
            main.startLifetime = 2.0f;

            var emission = ps.emission;
            emission.rateOverTime = 0;
        }
    }

    private void UpdateTrailWidth()
    {
        if (!isDrifting) return;

        float dynamicWidth = Mathf.Lerp(trailWidth, trailMaxWidth, driftTime / colorTransitionTime);
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.startWidth = dynamicWidth;
            trail.endWidth = dynamicWidth * 0.7f;

            float pulseAmount = Mathf.Sin(Time.time * 8f) * 0.15f + 1.0f;
            trail.widthMultiplier = 1.2f * pulseAmount;
        }
    }

    private void UpdateEffectColors()
    {
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

    private void EnableEffects()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (!ps.isPlaying) ps.Play();
            var emission = ps.emission;
            emission.rateOverTime = 0;
        }

        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.Clear();
            trail.enabled = true;
            trail.emitting = true;

            // Start with zero width and fade in
            trail.startWidth = 0;
            trail.endWidth = 0;

            // Start with zero alpha and fade in
            Color startColor = trail.startColor;
            Color endColor = trail.endColor;
            trail.startColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            trail.endColor = new Color(endColor.r, endColor.g, endColor.b, 0);
        }
    }

    private void SetEffectsInactive()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var emission = ps.emission;
            emission.rateOverTime = 0;
        }

        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.Clear();
            trail.enabled = false;
            trail.emitting = false;
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

        // Clear stored original values
        originalEmissionRates.Clear();
        originalStartWidths.Clear();
        originalEndWidths.Clear();
        originalStartColors.Clear();
        originalEndColors.Clear();

        trailAttachPoints = newAttachPoints;

        if (newParticlePrefab != null)
            particlePrefab = newParticlePrefab;

        if (newTrailMaterial != null)
            trailMaterial = newTrailMaterial;

        InitializeEffectSystems();
    }
}