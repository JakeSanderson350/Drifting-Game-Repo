using System.Collections.Generic;
using UnityEngine;

public class CarDriftController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<Tire> frontTires, backTires;

    [SerializeField] private Transform groundTrigger;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private Transform centerOfMass;

    [Header("Control")]
    public float throttle = 1.0f;
    public float throttleIncrement = 0.05f;
    private float defaultThrottle = 0.5f;
    public float screenUse = 0.8f;
    private bool isDead = false;

    [Header("Body")]
    public float tireRadius = 0.5f;
    public float drag = 1.0f;

    [Header("Engine")]
    public float driveForce = 10.0f;

    [Header("Steering")]
    public float maxAngularAcceleration = 9000.0f;
    public float maxVisualSteeringAngle = 40.0f;
    public float maxVisualSteeringSpeed = 10.0f;

    [Header("Drifting")]
    public float driftAngleThreshold = 5.0f;
    public float maxDriftAngle = 60.0f;
    public float transferSpeed = 10.0f;
    private float driftAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnEnable()
    {
        CarState.onCarDeath += GameOver;
    }

    private void OnDisable()
    {
        CarState.onCarDeath -= GameOver;
    }

    // Update is called once per frame
    void Update()
    {
        // update tire angles visually
        float wheelAngle = -Vector3.Angle(rb.linearVelocity.normalized, GetDriveDirection()) * Vector3.Cross(rb.linearVelocity.normalized, GetDriveDirection()).y;
        // clamp wheel angle
        wheelAngle = Mathf.Min(Mathf.Max(-maxVisualSteeringAngle, wheelAngle), maxVisualSteeringAngle);
        //Debug.Log(wheelAngle);

        PointWheelsAt(wheelAngle);
    }

    public float GetDriftAngle()
    {
        if (!IsGrounded())
        {
            return 0.0f;
        }

        return Vector3.Angle(rb.linearVelocity, GetDriveDirection()) * Vector3.Cross(rb.linearVelocity.normalized, GetDriveDirection()).y;
    }

    public float GetSlipRatio()
    {
        float carVelocity = rb.linearVelocity.magnitude;
        float tireAngularVelocity = 6.0f;

        float slipRatio = tireAngularVelocity * tireRadius - carVelocity;
        slipRatio /= Mathf.Abs(carVelocity);
        Debug.Log(slipRatio);

        return slipRatio;
    }

    public bool IsDrifting()
    {
        return Mathf.Abs(driftAngle) > driftAngleThreshold;
    }

    private void PointWheelsAt(float _targetAngle)
    {
        foreach (Tire tire in frontTires)
        {
            float currentAngle = tire.transform.localEulerAngles.y;
            float change = ((((_targetAngle - currentAngle) % 360) + 540) % 360) - 180;
            float newAngle = currentAngle + change * Time.deltaTime * maxVisualSteeringSpeed;
            tire.transform.localEulerAngles = new Vector3(0, newAngle, 0);
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        rb.centerOfMass = centerOfMass.localPosition;
        rb.AddForce(-GetDragForce() * rb.linearVelocity.normalized);

        // Check if grounded
        if (IsGrounded())
        {
            driftAngle = GetDriftAngle();

            // Engine
            UpdateEngine();

            // Steering
            rb.angularVelocity += -transform.up * GetSteeringAngularAcceleration() * Time.fixedDeltaTime;
            //Debug.DrawLine(transform.position, transform.position + rb.angularVelocity, Color.blue);
        }

        UpdateTireForce();
    }

    public bool IsGrounded()
    {
        return Physics.OverlapBox(groundTrigger.position, groundTrigger.localScale / 2, Quaternion.identity, groundLayerMask).Length > 0;
    }

    private void UpdateEngine()
    {
        if (!IsDrifting())
        {
            throttle = Mathf.Clamp01(throttle + throttleIncrement * Time.fixedDeltaTime);
        }
        else
        {
            throttle = Mathf.Clamp(throttle - throttleIncrement * Time.fixedDeltaTime, defaultThrottle, 1.0f);
        }

        rb.AddForce(GetDriveDirection() * (driveForce * throttle));
        Debug.DrawLine(transform.position, transform.position + GetDriveDirection() * (driveForce * throttle), Color.red);
    }

    private void UpdateTireForce()
    {
        //Update tire forces
        foreach (Tire tire in frontTires)
        {
            //tire.tireGripFactor = TireGripLookUpCurve(Mathf.Abs(driftAngle));

            tire.UpdateForces();

            rb.AddForceAtPosition(tire.GetForces(), tire.transform.position);
            Debug.DrawLine(tire.transform.position, tire.transform.position + tire.GetForces(), Color.green);
        }
        foreach (Tire tire in backTires)
        {
            tire.tireGripFactor = TireGripLookUpCurve(Mathf.Abs(driftAngle));

            tire.UpdateForces();

            rb.AddForceAtPosition(tire.GetForces(), tire.transform.position);
            Debug.DrawLine(tire.transform.position, tire.transform.position + tire.GetForces(), Color.green);
        }
    }

    private float TireGripLookUpCurve(float _tireAngle)
    {
        float tireGrip;

        if (_tireAngle < 10)
        {
            tireGrip = 0.9f;
        }
        else
        {
            tireGrip = 0.0075f * _tireAngle - 0.5f;
        }
        return tireGrip;
    }

    private float GetSteeringAngularAcceleration()
    {
        return GetSteering() * maxAngularAcceleration * Mathf.PI / 180;
    }

    private float GetSteering()
    {
        return Mathf.Clamp(TouchInput.centeredScreenPosition.x / screenUse, -1, 1);
    }

    private Vector3 GetDriveDirection()
    {
        return rb.transform.forward.normalized;
    }

    private float GetDragForce()
    {
        return Mathf.Pow(rb.linearVelocity.magnitude, 2) * drag;
    }

    private void GameOver()
    {
        isDead = true;
    }
}
