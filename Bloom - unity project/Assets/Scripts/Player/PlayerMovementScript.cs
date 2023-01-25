using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : PlayerBaseScript
{
    [Header("Movement parameters")]
    [SerializeField] float speed = 6;
    [SerializeField] float acceleration = 10;
    [SerializeField] float airAcceleration = 1;

    [SerializeField] float gravity = -29.46f;
    [SerializeField] float jumpForce = 500;
    [SerializeField] float jumpCooldown = 0.3f;

    [Header("Info")]
    public static bool isGrounded = false;
    public static bool completelyGrounded = false;
    public static bool isMoving = false;
    public bool canJump = true;

    Vector3 targetVelocity;

    public static bool canMove = true;
    public static bool isBathing = false;

    bool footstep = true;

    //Så man inte kan gå snabbare när man går snett
    float maxMagnitude = 1;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        canMove = true;
        maxMagnitude = new Vector3(1, 0, 0).magnitude;
    }

    private void Update()
    {
        if (!canMove) return;

        GroundCheck();

        Vector3 input = Vector3.zero;
        input += Input.GetAxisRaw("Horizontal") * transform.right;
        input += Input.GetAxisRaw("Vertical") * transform.forward;
        input = input.normalized * maxMagnitude;
        SetSpeed(input, speed);
        isMoving = (input != Vector3.zero);

        if (Input.GetKeyDown(KeyCode.Space)) Jump();

        if(isMoving && completelyGrounded && footstep)
        {
            footstep = false;
            AudioManager.current.PlayFootStep();
            Invoke(nameof(FootStepAgain), 0.4f);
        }
    }

    void FootStepAgain()
    {
        footstep = true;
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
    }

    void GroundCheck()
    {
        isGrounded = OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Ground"));
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.up, Vector3.down, out hit, 10, LayerMask.GetMask("Ground", "Water")))
        {
            if (hit.collider.gameObject.layer == 8) isBathing = true;
            else isBathing = false;
        }
        //isBathing = OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Water"));
        completelyGrounded = OverlapSphere(transform.position + new Vector3(0,0.49f, 0), 0.5f, LayerMask.GetMask("Ground"));

        Collider[] collectibles = new Collider[1];
        if(OverlapSphere(transform.position, 1f, LayerMask.GetMask("Collectible"), out collectibles))
        {
            for (int i = 0; i < collectibles.Length; i++)
            {
                cc.Collect(collectibles[i].GetComponent<CollectibleScript>());
            }         
        }

        Collider[] helathPickups = new Collider[1];
        if (OverlapSphere(transform.position, 1f, LayerMask.GetMask("HealthPickup"), out helathPickups))
        {
            for (int i = 0; i < helathPickups.Length; i++)
            {
                AudioManager.current.PlaySound(AudioManager.AudioNames.HeartSound);

                //är lat
                h.Damage(-1000);
                Destroy(helathPickups[0].gameObject);
            }
        }
    }

    void Jump()
    {
        if (!isGrounded || !canJump) return;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce);

        canJump = false;
        Invoke(nameof(CanJumpAgain), jumpCooldown);
    }

    void CanJumpAgain()
    {
        canJump = true;
    }

    bool OverlapSphere(Vector3 position, float radius, LayerMask mask)
    {
        Collider[] cols = Physics.OverlapSphere(position, radius, mask);

        if (cols.Length > 0) return true;
        else return false;
    }

    bool OverlapSphere(Vector3 position, float radius, LayerMask mask, out Collider[] cols)
    {
        cols = Physics.OverlapSphere(position, radius, mask);

        if (cols.Length > 0) return true;
        else return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.49f, 0), 0.5f);

    }
}
