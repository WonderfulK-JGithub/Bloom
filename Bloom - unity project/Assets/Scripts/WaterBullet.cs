using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    
    [SerializeField] GameObject particleTrail;
    [SerializeField] GameObject splashParticle;
    [SerializeField] float gravity;
    [SerializeField] LayerMask ignoreLayers;

    Rigidbody rb;
    SphereCollider col;
    Vector3 lastPos;

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

    
    void Splash(Collider _other)
    {
        Vector3 _hitPoint = _other.ClosestPointOnBounds(lastPos);

        Vector3 _direction = (_hitPoint - lastPos).normalized;

        Ray _ray = new Ray(lastPos, _direction);
        RaycastHit _hit;

        print(Physics.Raycast(_ray, out _hit));
        print(_hit.point.x + " " + _hit.point.y + " " + _hit.point.z);


        Debug.DrawRay(lastPos, _direction,Color.red);
        Debug.DrawRay(_hit.point, _hit.normal);

        //Debug.LogError("a");

        Transform _trans = Instantiate(splashParticle, _hitPoint + _hit.normal * 0.05f, Quaternion.identity).transform;
        _trans.up = _hit.normal;
        //_trans.SetParent(_other.transform);
        Destroy(_trans.gameObject, 1.5f);

        particleTrail.transform.SetParent(null);

        Destroy(gameObject);
        Destroy(particleTrail, 1f);
    }

    private void FixedUpdate() 
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

        lastPos = transform.position;
    }
}

public interface IWaterable
{
    void Water();
}