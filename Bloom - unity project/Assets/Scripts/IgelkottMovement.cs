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
    GameObject tagg;
    public float taggSpeed = 15;

    protected override void Start()
    {
        Transform[] objects = GetComponentsInChildren<Transform>();
        foreach (var obj in objects)
        {
            if (obj.GetComponent<tagg>())
            {
                tagg = obj.gameObject;
                tagg.GetComponent<tagg>().parent = this;
            }
        }

        tagg.SetActive(false);
        base.Start();
    }

    protected override void Movement()
    {
        lastatPlayer = atPlayer;
        atPlayer = distanceToPlayer < attackRange;

        if (chase)
        {
            if (!lastchase)
            {
                StopCoroutine(wander);
            }
            if (!atPlayer)
            {
                rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * System.Convert.ToInt32(!onGround);

                if (lastatPlayer)
                {
                    StopCoroutine(attack);
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
                wander = StartCoroutine(Wander());
            }
        }
    }
    IEnumerator Attack()
    {
        rb.angularVelocity = Vector3.zero;
        Quaternion startrot = transform.rotation;
        Quaternion targetrot = transform.rotation * Quaternion.Euler(0, 180, 0);
        float t = 0;
        while (t < 1)
        {
            rb.rotation = Quaternion.Lerp(startrot, targetrot, t);
            t += Time.deltaTime * attackRotationTime;
            yield return 0;
        }
        GameObject newtagg = Instantiate(tagg, transform.position - transform.forward, transform.rotation);
        newtagg.SetActive(true);
    }
    protected override void Rotation()
    {
        if (chase)
        {
            if (!atPlayer)
            {
                rb.angularVelocity = Vector3.zero;
                Quaternion fullRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
                rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
            }
        }
    }
}
