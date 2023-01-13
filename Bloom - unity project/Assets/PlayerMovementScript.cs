using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField] float speed = 6;
    [SerializeField] float acceleration = 10;
    [SerializeField] float airAcceleration = 1;

    [SerializeField] float gravity = -29.46f;
    [SerializeField] float jumpForce = 500;

    [Header("Info")]
    public static bool isGrounded = false;
    public static bool completelyGrounded = false;
    public static bool isMoving = false;

    Vector3 targetVelocity;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GroundCheck();

        Vector3 input = Vector3.zero;
        input += Input.GetAxisRaw("Horizontal") * transform.right;
        input += Input.GetAxisRaw("Vertical") * transform.forward;
        SetSpeed(input, speed);
        isMoving = (input != Vector3.zero);

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = Vector3.Lerp(rb.velocity, targetVelocity, (isGrounded ? acceleration : airAcceleration) * Time.fixedDeltaTime);
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        Gravity();

        if(!isMoving && completelyGrounded) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    void SetSpeed(Vector3 wishDir, float spd)
    {
        targetVelocity = spd * wishDir;
    }

    void Gravity()
    {
        if (!(completelyGrounded && !isMoving))
            rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
            //rb.AddForce(Vector3.up * gravity * rb.mass * Time.fixedDeltaTime);
    }

    void GroundCheck()
    {
        isGrounded = OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Ground"));
        completelyGrounded = OverlapSphere(transform.position + new Vector3(0,0.49f, 0), 0.5f, LayerMask.GetMask("Ground"));
    }

    void Jump()
    {
        if (!isGrounded) return;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce);
    }

    bool OverlapSphere(Vector3 position, float radius, LayerMask mask)
    {
        Collider[] cols = Physics.OverlapSphere(position, radius, mask);

        if (cols.Length > 0) return true;
        else return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.49f, 0), 0.5f);
    }
}
