using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] GameObject bullet;

    [SerializeField] float bulletSpeed = 15;

    [SerializeField] float bulletsPerSecond = 1;

    bool canShoot = true;

    [SerializeField] float bulletCost = 5;

    float _ammo = 100;
    float ammo
    {
        get { return _ammo; }
        set { value = Mathf.Clamp(value, 0, 100); if (waterSlider != null) waterSlider.value = value / 100f; _ammo = value; }
    }

    [SerializeField] Slider waterSlider;

    [SerializeField] float waterReloadSpeed;

    GunScript visual;

    Animator anim;


    private void Awake()
    {
        visual = GetComponent<GunScript>();
        anim = GetComponentInChildren<Animator>();

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canShoot || ammo <= 0 || PlayerHealthScript.isDead) return;

            visual.Fire();

            GameObject newBullet = Instantiate(bullet, barrel.position, barrel.rotation);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, LayerMask.GetMask("Ground")))
            {
                newBullet.GetComponent<WaterBullet>().SetVelocity((hit.point - barrel.position).normalized, bulletSpeed);
            }
            else
            {
                newBullet.GetComponent<WaterBullet>().SetVelocity((barrel.forward).normalized, bulletSpeed);
                //newBullet.GetComponent<WaterBullet>().SetVelocity((Camera.main.transform.forward * 100 - barrel.position).normalized, bulletSpeed);
            }

            Physics.IgnoreCollision(newBullet.GetComponent<SphereCollider>(), GetComponentInChildren<CapsuleCollider>());

            ammo -= bulletCost;
            ammo = Mathf.Clamp(ammo, 0, 100);

            AudioManager.current.PlaySound(AudioManager.AudioNames.WaterSpray);

            canShoot = false;
            Invoke(nameof(ActivateShoot), 1 / bulletsPerSecond);
        }

        anim.SetBool("isReloading", false);

        if (Input.GetMouseButton(1))
        {
            RaycastHit hitWater;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitWater, 2, LayerMask.GetMask("Water", "Ground")))
            {
                if(hitWater.collider.gameObject.layer == 4)
                {
                    if (Input.GetMouseButtonDown(1) && ammo != 100)
                    {
                        AudioManager.current.PlaySound(AudioManager.AudioNames.WaterFill);
                    }

                    anim.SetBool("isReloading", true);
                    ammo += waterReloadSpeed * Time.deltaTime;
                }
            }
        }
    }

    void ActivateShoot()
    {
        canShoot = true;
    }
}
