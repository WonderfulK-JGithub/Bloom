using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float airAcceleration;

    [SerializeField] float friction;

    Vector3 velocity;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 input = Vector3.zero;
        input += Input.GetAxisRaw("Horizontal") * transform.right;
        input += Input.GetAxisRaw("Vertical") * transform.forward;
        Accelerate(input, acceleration);

        rb.MovePosition(rb.position + velocity);
        Debug.Log(velocity);
    }

    void Accelerate(Vector3 wishDir, float accel)
    {
        float currentspeed = Vector3.Dot(velocity, wishDir);
        if (currentspeed == 0) currentspeed = 1;
        velocity += accel * wishDir * currentspeed;
    }
}
