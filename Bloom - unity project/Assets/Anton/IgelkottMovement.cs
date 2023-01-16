using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgelkottMovement : enemymovement
{
    [Header("Hedgehog specific ---------")]
    public float attackRange = 3;
    bool atPlayer;

    protected override void Movement()
    {
        atPlayer = distanceToPlayer < attackRange;

        if (chase)
        {
            if (!lastchase)
            {
                StopCoroutine(wander);
            }

            rb.velocity = (transform.forward * moveSpeed) + Physics.gravity * System.Convert.ToInt32(!onGround) * System.Convert.ToInt32(!atPlayer);
        }
        else
        {
            if (lastchase)
            {
                wander = StartCoroutine(Wander());
            }
        }
    }
}
