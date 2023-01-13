using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitMovement : enemymovement
{
    [Space]
    [Header("Rabbit specific ---------")]
    public float jumpBoost = 10;
    public float jumpCooldown = 2.5f;
    public float gravityMultiplier = 2;

    float secsSinceLastJump = 0;

    protected override void Movement()
    {
        secsSinceLastJump += Time.deltaTime;

        if (secsSinceLastJump > jumpCooldown && onGround)
        {
            secsSinceLastJump = 0;
            Jump();
        }
    }

    protected override Vector3 Gravity()
    {
        return base.Gravity() * gravityMultiplier;
    }

    void Jump()
    {
        rb.velocity += new Vector3(0, jumpBoost, 0);
    }
}
