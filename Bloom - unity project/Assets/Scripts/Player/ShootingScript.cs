using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] GameObject bullet;

    [SerializeField] float bulletSpeed = 15;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = Instantiate(bullet, barrel.position, barrel.rotation);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, LayerMask.GetMask("Ground")))
            {
                newBullet.GetComponent<WaterBullet>().SetVelocity((hit.point - barrel.position).normalized, bulletSpeed);
            }
            else
            {
                newBullet.GetComponent<WaterBullet>().SetVelocity((Camera.main.transform.forward * 100 - barrel.position).normalized, bulletSpeed);
            }

            Physics.IgnoreCollision(newBullet.GetComponent<SphereCollider>(), GetComponentInChildren<CapsuleCollider>());

        }
    }
}
