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
            if (detectionRange == 0)
            {
                Jump();
            }
            else
            {
                Invoke("Jump", 0.5f);
            }
            
        }
        else if (distanceToPlayer <= attackRange && secsSinceLastDamage > damageCooldown && detectionRange > 0)
        {
            secsSinceLastDamage = 0;
            animator.SetTrigger("attack");
            DamagePlayer(30);
        }



        if (lastchase && !chase && detectionRange > 0)
        {
            wander = StartCoroutine(Wander());
        }
        else if (!lastchase && chase)
        {
            utropstecken.SetActive(true);
            AudioManager.current.PlaySound(AudioManager.AudioNames.Alerted,transform.position);
            animator.SetBool("wander", false);
            if (wander != null)
            {
                StopCoroutine(wander);
                //rb.velocity = Gravity();
            }
        }

    }

    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator Wander()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float t = 0;
            float randomRotation = UnityEngine.Random.Range(0f, 360f);

            if (Mathf.Abs(transform.position.x - startPos.x) > 10 || Mathf.Abs(transform.position.z - startPos.z) > 10)
            {
                randomRotation = Quaternion.LookRotation(startPos).y + 180;
            }
            while (t < 1)
            {
                rb.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, randomRotation, 0)), t);
                t += Time.deltaTime;
                yield return 0;
            }

            float secs = UnityEngine.Random.Range(1f, 3f);
            float time = 0;
            animator.SetBool("wander", true);
            while (time < secs && !brake)
            {
                rb.velocity = (transform.forward * moveSpeed) + Gravity() * Convert.ToInt32(!onGround);
                rb.angularVelocity = new Vector3(0, 0, 0);
                time += Time.deltaTime;
                yield return 0;
            }
            animator.SetBool("wander", false);
            //rb.velocity = Gravity();
        }
    }

    protected override Vector3 Gravity()
    {
        return base.Gravity() * gravityMultiplier;
    }

    protected override void Rotation()
    {
        if (animator.GetBool("hoppar") == false)
        {
            base.Rotation();
        }
    }

    void Jump()
    {
        rb.angularVelocity = Vector3.zero;
        rb.velocity += new Vector3((transform.forward * moveSpeed * Convert.ToInt32(chase)).x, jumpBoost, (transform.forward * moveSpeed * Convert.ToInt32(chase)).z);
        AudioManager.current.PlaySound(AudioManager.AudioNames.KaninHopp, transform.position);
    }

    protected override IEnumerator Transformation()
    {
        moveSpeed = 0;
        jumpCooldown = 0.1f;
        gravityMultiplier = 5;
        jumpBoost = 15;
        return base.Transformation();
    }

    protected override void OnCollisionEnter(Collision collision) {  }
    protected override void OnCollisionStay(Collision collision) { base.OnCollisionStay(collision); animator.SetBool("hoppar", false); }
    protected override void OnCollisionExit(Collision collision) { base.OnCollisionExit(collision); animator.SetBool("hoppar", true); }
}
