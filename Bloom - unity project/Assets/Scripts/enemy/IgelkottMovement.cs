using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgelkottMovement : enemymovement
{
    [Header("Hedgehog specific ---------")]
    public float attackRange = 8;
    public float attackRotationTime = 2;
    bool atPlayer;
    bool lastatPlayer;
    Coroutine attack;
    public GameObject tagg;
    public float taggSpeed = 15;
    public float attacksDelay = 0.2f;
    Quaternion fullRot;
    Animator animator;
    bool attacking = false;
    public float taggLifetime = 3;
    public Transform kropp;
    Vector3 kroppPos;
    protected override void Start()
    {

        kroppPos = kropp.localPosition;
        animator = GetComponentInChildren<Animator>();
        
        base.Start();
    }

    protected override void Movement()
    {
        lastatPlayer = atPlayer;
        atPlayer = distanceToPlayer < attackRange;
        if (!attacking)
        {
            animator.SetBool("walk", true);
        }

        if (chase)
        {
            if (!lastchase)
            {
                utropstecken.SetActive(true);
                AudioManager.current.PlaySound(AudioManager.AudioNames.Alerted,transform.position);
                StopCoroutine(wander);
                kropp.localPosition = kroppPos;
            }
            if (!atPlayer)
            {
                rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * System.Convert.ToInt32(!onGround);

                if (lastatPlayer && detectionRange > 0)
                {
                    if (attack != null) { StopCoroutine(attack); }
                    
                    kropp.localPosition = kroppPos;
                    transform.rotation *= Quaternion.Euler(new Vector3(0, 180, 0));
                }
            }
            else
            {
                if (!lastatPlayer)
                {
                    attack = StartCoroutine(Attack());
                }
            }
        }
        else
        {
            if (lastchase)
            {
                animator.SetBool("walk", true);
                wander = StartCoroutine(Wander());
                if (attack != null)
                {
                    StopCoroutine(attack);
                    kropp.localPosition = kroppPos;
                    attacking = false;
                    animator.SetBool("walk", true);

                }
                transform.rotation *= Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }
    }

    protected override IEnumerator Wander()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float t = 0;
            float randomRotation = Random.Range(0f, 360f);

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

            float secs = Random.Range(1f, 3f);
            float time = 0;

            animator.SetBool("walk", true);

            while (time < secs && !brake)
            {
                rb.velocity = (transform.forward * moveSpeed) + Gravity() * System.Convert.ToInt32(!onGround);
                rb.angularVelocity = new Vector3(0, 0, 0);
                time += Time.deltaTime;
                yield return 0;
            }

            rb.velocity = Gravity();
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        animator.SetBool("walk", false);
        while (true)
        {
            animator.SetTrigger("attack");
            rb.angularVelocity = Vector3.zero;
            Quaternion startrot = transform.rotation;
            Quaternion targetrot = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y + 180 ,rb.rotation.z)); 
            float t = 0;
            Vector3 originpos = kropp.localPosition;

            while (t < 1)
            {
                //kropp.localPosition = originpos + new Vector3(0, t * 0.5f, 0);
                rb.angularVelocity = Vector3.zero;
                rb.rotation = Quaternion.Lerp(startrot, targetrot, t);
                t += Time.deltaTime * attackRotationTime;
                yield return 0;
            }

            AudioManager.current.PlaySound(AudioManager.AudioNames.Tagg, transform.position);

            for (int i = -1; i < 2; i ++)
            {
                GameObject newtagg = Instantiate(tagg, transform.position  - transform.forward + (-transform.right * transform.lossyScale.x * i), transform.rotation * Quaternion.Euler(new Vector3(0, i * 45, 0)));
                newtagg.SetActive(true);
                newtagg.GetComponent<tagg>().parent = this;
            }

            while (t > 0)
            {
                //kropp.localPosition = originpos + new Vector3(0, t * 0.5f, 0);
                t -= Time.deltaTime * attackRotationTime;
                yield return 0;
            }

            yield return new WaitForSeconds(attacksDelay);
        }

    }
    
    protected override void Rotation()
    {
        if (chase)
        {
            fullRot = Quaternion.LookRotation((target.position - transform.position).normalized);

            if (!atPlayer)
            {
                rb.angularVelocity = Vector3.zero;
                Quaternion fullRotLerp = Quaternion.Lerp(transform.rotation, fullRot, Time.deltaTime * rotationSpeed);
                rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRotLerp.eulerAngles.y, rb.rotation.z));
            }
        }
    }
}
