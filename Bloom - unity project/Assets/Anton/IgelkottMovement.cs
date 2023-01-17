using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgelkottMovement : enemymovement
{
    [Header("Hedgehog specific ---------")]
    public float attackRange = 3;
    bool atPlayer;
    bool lastatPlayer;

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
    protected override void Rotation()
    {
        if (chase)
        {
            if (atPlayer)
            {
                rb.angularVelocity = Vector3.zero;
                Quaternion fullRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
                rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
                Quaternion fullRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
                rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
            }
            
        }
    }
}
