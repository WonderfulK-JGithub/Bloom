using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    
    [SerializeField] GameObject particleTrail;

    [SerializeField] float gravity;

    Rigidbody rb;

    private void Awake()
    {
        Destroy(gameObject, 20f);
        rb = GetComponent<Rigidbody>();
    }

    public void SetVelocity(Vector3 _direction, float _speed)
    {
        rb.velocity = _direction * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Component _component = other.GetComponent(typeof(IWaterable));
        if(_component != null)
        {
            (_component as IWaterable).Water();
        }

        Splash();
    }

    void Splash()
    {
        particleTrail.transform.SetParent(null);

        Destroy(gameObject);
        Destroy(particleTrail, 1f);
    }

    private void FixedUpdate() 
    {
        //Gravitation - Max
        rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
    }
}

public interface IWaterable
{
    void Water();
}