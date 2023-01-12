using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    
    [SerializeField] GameObject particleTrail;


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
}

public interface IWaterable
{
    void Water();
}