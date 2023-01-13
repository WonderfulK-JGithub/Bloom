using System;
using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour, IWaterable
{
    public Transform target;
    public float rotationSpeed = 18;
    public float moveSpeed = 4.5f;
    protected Rigidbody rb;
    private bool chase = true;
    protected bool onGround = false;
    public float detectionRange = 10;
    Coroutine wander;
    [HideInInspector] public bool brake = false;
    public float hp = 1;
    protected float distanceToPlayer;
    protected bool lastchase = true;
    protected bool hasTransformed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    protected virtual void Update()
    {
        lastchase = chase;

        chase = false;
        distanceToPlayer = Vector3.Distance(transform.position + (transform.up * transform.lossyScale.y / 2), target.position);

        RaycastHit hit;
        Debug.DrawRay(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position));
        if (distanceToPlayer < detectionRange)
        {
            if (Physics.Raycast(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position), out hit, detectionRange))
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

            rb.angularVelocity = Vector3.zero;
            Quaternion fullRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
            rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
        }
        else
        {
            if (lastchase)
            {
                wander = StartCoroutine(Wander());
            }
            //rb.velocity = Gravity();
            //rb.angularVelocity = new Vector3(0, 0, 0);
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
                GetComponentInChildren<MeshRenderer>().material.color = new Color(1 - t, t, 0, 1);

                t += Time.deltaTime;
                yield return 0;
            }
        }
        

    }
    protected virtual Vector3 Gravity() { return Physics.gravity; }

    public void Water()
    {
        hp -= 0.1f;
        print("HP kvar: " + (hp * 100).ToString());
    }
    IEnumerator Wander()
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
}
