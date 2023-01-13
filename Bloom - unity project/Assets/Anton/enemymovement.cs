using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed;
    public float moveSpeed;
    Rigidbody rb;
    public bool chase;
    bool onGround = false;
    public float detectionRange = 10;
    public PhysicMaterial[] materials;
    Coroutine wander;
    public bool brake = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        bool lastchase = chase;

        chase = false;

        RaycastHit hit;
        Debug.DrawRay(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position));
        if (Vector3.Distance(transform.position + (transform.up * transform.lossyScale.y / 2), target.position) < detectionRange)
        {
            if (Physics.Raycast(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position), out hit, detectionRange))
            {
                if (hit.transform == target)
                {
                    chase = true;
                }
            }
        }

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
            //rb.velocity = Physics.gravity;
            //rb.angularVelocity = new Vector3(0, 0, 0);
        }
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
                rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * Convert.ToInt32(!onGround);
                rb.angularVelocity = new Vector3(0, 0, 0);
                time += Time.deltaTime;
                yield return 0;
            }
            
            rb.velocity = Physics.gravity;
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
