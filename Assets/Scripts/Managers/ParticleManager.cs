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
    [SerializeField] private float trailWidth = 2f;
    [SerializeField] private float trailMaxWidth = 4f;
    [SerializeField] private float trailHeightOffset = 0.01f;
    [SerializeField] private float trailTime = 1f;

    [Header("Particle Configuration")]
    [SerializeField] private float particleSpawnRadius = 1.5f;
    [SerializeField] private float particleSpawnHeightOffset = 0.2f;

    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();

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
            CreateParticleSystem(attachPoint);
            CreateTrailRenderer(attachPoint);
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

    private void CreateParticleSystem(Transform attachPoint)
    {
        GameObject particleObj = Instantiate(particlePrefab, attachPoint.position, Quaternion.identity, transform);
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
        main.startLifetime = 1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = particleSpawnRadius;
        shape.position = new Vector3(0, particleSpawnHeightOffset, 0);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = 0.5f;
    }

    private void ConfigureTrailRenderer(TrailRenderer trail)
    {
        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = trailWidth * 0.5f;
        trail.minVertexDistance = 0.2f;

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
        foreach (TrailRenderer trail in trailRenderers)
        {
            Transform attachPoint = trail.transform.parent;
            Vector3 raycastOrigin = attachPoint.position;

            if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 10f, roadLayerMask))
            {
                trail.transform.position = hit.point + (Vector3.up * trailHeightOffset);
                trail.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
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
            trail.endWidth = dynamicWidth * 0.5f;
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
        }

        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.enabled = active;
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

        trailAttachPoints = newAttachPoints;

        if (newParticlePrefab != null)
            particlePrefab = newParticlePrefab;

        if (newTrailMaterial != null)
            trailMaterial = newTrailMaterial;

        InitializeEffectSystems();
    }
}