using System;
using System.Collections;
using UnityEngine;

public class RabbitMovement : enemymovement
{
    [Space]
    [Header("Rabbit specific ---------")]
    public float jumpBoost = 10;
    public float jumpCooldown = 2.5f;
    public float damageCooldown = 5;
    public float gravityMultiplier = 2;
    public float attackRange = 3;

    float secsSinceLastJump = 0;
    float secsSinceLastDamage = 0;
    Animator animator;

    protected override void Start()
    {
        animator = GetComponentInChildren<Animator>();
        base.Start();
    }
    protected override void Movement()
    {
        secsSinceLastJump += Time.deltaTime;
        secsSinceLastDamage += Time.deltaTime;

        //print((transform.forward * moveSpeed * Convert.ToInt32(chase)));
        if (secsSinceLastJump > jumpCooldown && onGround && ((distanceToPlayer > attackRange && chase) || detectionRange == 0))
        {
            secsSinceLastJump = 0;
            Jump();
        }
        else if (distanceToPlayer <= attackRange && secsSinceLastDamage > damageCooldown && detectionRange > 0)
        {
            secsSinceLastDamage = 0;
            animator.SetTrigger("attack");
            DamagePlayer(30);
        }



        if (lastchase && !chase && detectionRange > 0)
        {
            animator.SetBool("wander", true);
            wander = StartCoroutine(Wander());
        }
        else if (!lastchase && chase)
        {
            animator.SetBool("wander", false);
            if (wander != null)
            {
                StopCoroutine(wander);
                rb.velocity = Gravity();
            }
        }

    }

    protected override Vector3 Gravity()
    {
        return base.Gravity() * gravityMultiplier;
    }

    void Jump()
    {
        animator.SetBool("hoppar", true);
        rb.velocity += new Vector3((transform.forward * moveSpeed * Convert.ToInt32(chase)).x, jumpBoost, (transform.forward * moveSpeed * Convert.ToInt32(chase)).z);
    }

    protected override IEnumerator Transformation()
    {
        moveSpeed = 0;
        jumpCooldown = 0.1f;
        gravityMultiplier = 5;
        jumpBoost = 15;
        return base.Transformation();
    }

    protected override void OnCollisionEnter(Collision collision) { animator.SetBool("hoppar", false); }
}
