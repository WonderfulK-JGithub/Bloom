using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBullet : WaterBullet
{
    //[SerializeField] LayerMask playerLayer;
    [SerializeField] int damage;

    protected override void FixedUpdate()
    {
        //Gravitation - Max
        rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);


        Collider[] _others = Physics.OverlapSphere(transform.position, col.radius * transform.localScale.x,~ignoreLayers);

        if (_others.Length > 0)
        {
            Splash(_others[0]);
            print(_others[0].name);
            if (_others[0].CompareTag("Player"))
            {
                DamagePlayer(_others[0]);
            }
        }

        if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit _hit))
        {
            lastHit = _hit;
        }
    }

    void DamagePlayer(Collider _other)
    {
        _other.GetComponentInParent<PlayerHealthScript>().Damage(damage);
    }
}
