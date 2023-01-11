using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField] float maxVelocity;
    [SerializeField] float acceleration;
    [SerializeField] float airAcceleration;

    [SerializeField] float friction;

    Rigidbody rb;

    public bool isGrounded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GroundCheck();

        //Clamp velocity
        //Vector3 newVelocity = velocity.normalized;
        //newVelocity *= maxVelocity;
        //velocity = newVelocity;
    }

    private void FixedUpdate()
    {
        Vector3 input = Vector3.zero;
        input += Input.GetAxisRaw("Horizontal") * transform.right;
        input += Input.GetAxisRaw("Vertical") * transform.forward;
        Accelerate(input, acceleration);

        if (input == Vector3.zero) Friction();
    }

    void Accelerate(Vector3 wishDir, float accel)
    {
        rb.velocity += accel * wishDir * Time.fixedDeltaTime;
    }

    void Friction()
    {
        if (!isGrounded) return;
    }

    void GroundCheck()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Ground"));

        if(cols.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
