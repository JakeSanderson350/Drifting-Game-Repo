using UnityEngine;

public class CarGravity : MonoBehaviour
{
    public float acceleration = 9.8f;
    public Vector3 direction = Vector3.down;
    public float maxAngle = 30.0f;

    [SerializeField] Rigidbody rb;
    [SerializeField] CarDriftController car;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (car.IsGrounded() || Vector3.Angle(Vector3.down, -transform.up.normalized) < maxAngle)
        {
            direction = -transform.up.normalized;
        }
        //rb.linearVelocity += direction * acceleration * Time.fixedDeltaTime;
        rb.AddForce(direction * acceleration);
    }
}
