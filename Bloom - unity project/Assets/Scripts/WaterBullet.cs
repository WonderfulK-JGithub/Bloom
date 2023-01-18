using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    
    [SerializeField] GameObject particleTrail;
    [SerializeField] GameObject splashParticle;
    [SerializeField] float splashTime = 1.5f;
    [SerializeField] AudioManager.AudioNames splashSound;

    public float gravity;
    public LayerMask ignoreLayers;

    protected Rigidbody rb;
    protected SphereCollider col;
    protected RaycastHit lastHit;

    private void Awake()
    {
        Destroy(gameObject, 20f);
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
    }

    public void SetVelocity(Vector3 _direction, float _speed)
    {
        rb.velocity = _direction * _speed;
    }

    
    protected void Splash(Collider _other)
    {
        Transform _trans = Instantiate(splashParticle, lastHit.point, Quaternion.identity).transform;
        _trans.up = lastHit.normal;
        _trans.SetParent(_other.transform);
        Destroy(_trans.gameObject, splashTime);

        particleTrail.transform.SetParent(null);

        Destroy(gameObject);
        Destroy(particleTrail, 1f);

        AudioManager.current.PlaySound(splashSound);
    }

    protected virtual void FixedUpdate() 
    {
        //Gravitation - Max
        rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
        

        Collider[] _others = Physics.OverlapSphere(transform.position, col.radius * transform.localScale.x, ~ignoreLayers);

        if(_others.Length > 0)
        {
            Splash(_others[0]);

            for (int i = 0; i < _others.Length; i++)
            {
                Component _component = _others[i].GetComponent(typeof(IWaterable));
                if (_component != null)
                {
                    (_component as IWaterable).Water();
                }
            }
            
        }

        if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit _hit))
        {
            lastHit = _hit;
        }

        
    }
}

public interface IWaterable
{
    void Water();
}