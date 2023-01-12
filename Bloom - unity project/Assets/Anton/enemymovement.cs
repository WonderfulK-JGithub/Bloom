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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        

        chase = false;

        RaycastHit hit;
        if (Vector3.Distance(transform.position, target.position) < detectionRange)
        {
            if (Physics.Raycast(transform.position, (target.position - transform.position), out hit, detectionRange))
            {
                if (hit.transform == target)
                {
                    chase = true;
                }
            }
        }

        if (chase)
        {
            rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * Convert.ToInt32(!onGround);

            Quaternion fullRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
            rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
        }
    }

    private void OnCollisionEnter(Collision collision)
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
