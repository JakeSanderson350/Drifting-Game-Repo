using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ParticleManager : MonoBehaviour
{
    [Header("Particle Properties")]
    [SerializeField] private float baseEmissionRate;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float gradientPositions;

    [SerializeField] private GameObject car;
  //  [SerializeField] private List<Tire> tires;
    
    private CarDriftController driftController;
    private List<ParticleSystem> particleSystems;
    private ScoreManager scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        scoreManager = GetComponent<ScoreManager>();
        driftController = car.GetComponent<CarDriftController>();
        particleSystems = car.GetComponentsInChildren<ParticleSystem>().ToList();

    }

    // Update is called once per frame
    void Update()
    {
        HandleDriftStart();

    }
    void HandleDriftStart()
    {
        if (driftController.IsDrifting())
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                var emission = particleSystem.emission;
                //particleSystem.duration = baseEmissionRate + Time.deltaTime;
                //Debug.Log("Current Emission Rate: " + semission.rateOverTime);
                emission.enabled = true;
            }
        }
        else if (!driftController.IsDrifting())
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                var emission = particleSystem.emission;
                emission.enabled = false;
            }
        }
    }

    void HandleDriftLength()
    {

    }
}


