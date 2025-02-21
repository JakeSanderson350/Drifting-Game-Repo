using System.Collections.Generic;
using UnityEngine;

public class CarDriftController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<Tire> frontTires, backTires;

    [SerializeField] private Transform groundTrigger;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private Transform centerOfMass;

    // Control
    public float throttle = 1.0f;
    public float screenUse = 0.8f;

    // Body
    public float drag = 1.0f;

    // Engine
    public float driveForce = 10.0f;

    // Steering
    public float maxAngularAcceleration = 30.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // update tire angles visually
    }

    private void FixedUpdate()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.AddForce(-GetDragForce() * rb.linearVelocity.normalized);

        // Check if grounded
        if (IsGrounded())
        {
            // Engine
            rb.AddForce(GetDriveDirection() * (driveForce * throttle));
            Debug.DrawLine(transform.position, transform.position + GetDriveDirection() * (driveForce * throttle), Color.red);

            // Steering
            rb.angularVelocity += -transform.up * GetSteeringAngularAcceleration() * Time.fixedDeltaTime;
            Debug.DrawLine(transform.position, transform.position + rb.angularVelocity, Color.blue);
        }

        UpdateSuspensionForce();
    }

    private bool IsGrounded()
    {
        return Physics.OverlapBox(groundTrigger.position, groundTrigger.localScale / 2, Quaternion.identity, groundLayerMask).Length > 0;
    }

    private void UpdateSuspensionForce()
    {
        //Update tire forces
        foreach (Tire tire in frontTires)
        {
            tire.UpdateForces();

            rb.AddForceAtPosition(tire.GetForces(), tire.transform.position);
            Debug.DrawLine(tire.transform.position, tire.transform.position + tire.GetForces(), Color.green);
        }
        foreach (Tire tire in backTires)
        {
            tire.UpdateForces();

            rb.AddForceAtPosition(tire.GetForces(), tire.transform.position);
            Debug.DrawLine(tire.transform.position, tire.transform.position + tire.GetForces(), Color.green);
        }
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
}
