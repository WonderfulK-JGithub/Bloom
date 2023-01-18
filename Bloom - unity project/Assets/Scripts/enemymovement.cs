using System;
using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour, IWaterable
{
    protected Transform target;
    public float rotationSpeed = 18;
    public float moveSpeed = 4.5f;
    protected Rigidbody rb;
    protected bool chase = true;
    protected bool onGround = false;
    public float detectionRange = 10;
    protected Coroutine wander;
    [HideInInspector] public bool brake = false;
    public float hp = 100;
    protected float distanceToPlayer;
    protected bool lastchase = true;
    protected bool hasTransformed = false;
    public SkinnedMeshRenderer color;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    void Update()
    {
        lastchase = chase;

        chase = false;
        distanceToPlayer = Vector3.Distance(transform.position + (transform.up * transform.lossyScale.y / 2), target.position);

        RaycastHit hit;
        Debug.DrawRay(transform.position  + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position));
        if (distanceToPlayer < detectionRange)
        {
            int mask = (1 << 9);
            mask += (1 << 4);
            mask = ~mask;
            if (Physics.Raycast(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position), out hit, detectionRange, mask) && !PlayerMovementScript.isBathing)
            {
                if (hit.transform == target)
                {
                    chase = true;
                }
            }
        }

        if (hp <= 0)
        {
            StartCoroutine(Transformation());
        }

        Movement();
        Rotation();
    }

    private void FixedUpdate()
    {
        rb.velocity += Gravity() * Time.fixedDeltaTime;
    }

    protected virtual void Movement()
    {

        if (chase)
        {
            if (!lastchase)
            {
                StopCoroutine(wander);
            }

            rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * Convert.ToInt32(!onGround);
        }
        else
        {
            if (lastchase)
            {
                wander = StartCoroutine(Wander());
            }
        }
    }

    protected virtual void Rotation()
    {

        if (chase)
        {
            rb.angularVelocity = Vector3.zero;
            Quaternion fullRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
            rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
        }
    }

    protected virtual IEnumerator Transformation()
    {
        if (!hasTransformed)
        {
            hasTransformed = true;
            detectionRange = 0;

            float t = 0;
            Vector3 scale = transform.localScale;
            Vector3 targetScale = transform.localScale / 1.5f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(scale, targetScale, t);

                t += Time.deltaTime;
                yield return 0;
            }
        }
        

    }
    protected virtual Vector3 Gravity() { return Physics.gravity; }

    public void Water()
    {
        DamageEnemy(7, 14);
    }
    public void DamageEnemy(float min, float max)
    {
        hp -= UnityEngine.Random.Range(min, max);
        if (hp >= 0)
        {
            color.material.SetFloat("_OilLevel", 1 - (hp / 100f));
        }
        else
        {
            color.material.SetFloat("_OilLevel", 1);
        }
    }
    protected virtual IEnumerator Wander()
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
            while (time < secs && !brake)
            {
                rb.velocity = (transform.forward * moveSpeed) + Gravity() * Convert.ToInt32(!onGround);
                rb.angularVelocity = new Vector3(0, 0, 0);
                time += Time.deltaTime;
                yield return 0;
            }

            rb.velocity = Gravity();
        }
    }

    public void DamagePlayer(int avgDamage)
    {
        if (target.GetComponent<PlayerHealthScript>())
        {
            target.GetComponent<PlayerHealthScript>().Damage(UnityEngine.Random.Range(avgDamage - 5, avgDamage + 6));
        }
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && detectionRange > 0)
        {
            DamagePlayer(10);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onGround = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*
        if (other.gameObject.layer == 4)
        {
            DamageEnemy(Time.deltaTime, Time.deltaTime * 2);
        }
        */
    }
}
