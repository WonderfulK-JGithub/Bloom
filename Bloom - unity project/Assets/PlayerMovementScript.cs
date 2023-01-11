using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float airAcceleration;

    [SerializeField] float gravity = -9.82f;

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

        Gravity();
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = Vector3.Lerp(rb.velocity, targetVelocity, (isGrounded ? acceleration : airAcceleration) * Time.fixedDeltaTime);
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
    }

    void SetSpeed(Vector3 wishDir, float spd)
    {
        targetVelocity = spd * wishDir;
    }

    void Gravity()
    {
        if(!(completelyGrounded && !isMoving))
            rb.AddForce(Vector3.up * gravity * rb.mass * Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Ground"));
        completelyGrounded = OverlapSphere(transform.position + new Vector3(0,0.49f, 0), 0.5f, LayerMask.GetMask("Ground"));
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
