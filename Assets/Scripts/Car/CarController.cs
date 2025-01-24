using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody carRB;

    [SerializeField]
    private List<Tire> frontTires, backTires;
    [SerializeField]
    public float tireTurnSpeed = 1;
    private float tireAngle = 0.0f;

    private float accelerationInput;
    private float brakeInput;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        accelerationInput = Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;

        brakeInput = Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            foreach (Tire tire in frontTires)
            {
                if (tireAngle > -90)
                {
                    tire.transform.Rotate(0, -tireTurnSpeed, 0);
                    tireAngle -= tireTurnSpeed;
                }
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            foreach (Tire tire in frontTires)
            {
                if (tireAngle < 90)
                {
                    tire.transform.Rotate(0, tireTurnSpeed, 0);
                    tireAngle += tireTurnSpeed;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Check collision
        CollisionResolution();

        //Update tire forces
        foreach (Tire tire in frontTires)
        {
            tire.UpdateForces(accelerationInput, brakeInput, transform.forward);

            //carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
        foreach (Tire tire in backTires)
        {
            tire.UpdateForces(accelerationInput, brakeInput, transform.forward);

            //carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
    }

    private void CollisionResolution()
    {
        //PlaneCollider[] planes = FindObjectsOfType<PlaneCollider>();

        //foreach (Sphere tire in tireColliders)
        //{
        //    foreach (PlaneCollider plane in planes)
        //    {
        //        Vector3 tireVelocity = carRB.GetVelocityAtPoint(tire.transform.position);
        //        tireVelocity = new Vector3(0.0f, tireVelocity.y, 0.0f);

        //        CollisionInfo info = GetCollisionInfo(tire, plane, tireVelocity);

        //        //Apply force to rigidbody
        //        if (info.IsColliding)
        //        {
        //            //Get restitution velocity of tire
        //            VectorDeltas deltaVelelocity = ResolveVelocity(info);
        //            Vector3 tireDeltaVelocity = deltaVelelocity.s1;

        //            Vector3 collisionForce = (1 / tire.invMass) * (/*tireVelocity +*/ tireDeltaVelocity); //F = mv
        //            carRB.AddForce(collisionForce, tire.transform.position);
        //            Debug.DrawLine(tire.transform.position, tire.transform.position + collisionForce, Color.magenta);
        //        }
        //    }
        //}
    }
}
