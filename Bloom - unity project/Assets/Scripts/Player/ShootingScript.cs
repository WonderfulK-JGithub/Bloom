using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    [Header("Shooting")]
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

    [Header("Reloading")]
    [SerializeField] Slider waterSlider;

    [SerializeField] float waterReloadSpeed;

    [SerializeField] Image tilNextShotLoading;

    [SerializeField] GameObject reloadIcon;

    GunScript visual;

    Animator anim;


    public virtual void Awake()
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
            WaterBullet bul = newBullet.GetComponent<WaterBullet>();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, LayerMask.GetMask("Ground")))
            {
                bul.SetVelocity((hit.point - barrel.position).normalized, bulletSpeed);
            }
            else
            {
                bul.SetVelocity((barrel.forward).normalized, bulletSpeed);
                //newBullet.GetComponent<WaterBullet>().SetVelocity((Camera.main.transform.forward * 100 - barrel.position).normalized, bulletSpeed);
            }

            Physics.IgnoreCollision(newBullet.GetComponent<SphereCollider>(), GetComponentInChildren<CapsuleCollider>());

            ammo -= bulletCost;
            ammo = Mathf.Clamp(ammo, 0, 100);

            AudioManager.current.PlaySound(AudioManager.AudioNames.WaterSpray);

            canShoot = false;
            StartCoroutine(tilCanShoot());
        }

        anim.SetBool("isReloading", false);
        reloadIcon.SetActive(false);

        RaycastHit hitWater;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitWater, 3, LayerMask.GetMask("Water", "Ground")))
        {
            if (hitWater.collider.gameObject.layer == 4)
            {
                reloadIcon.SetActive(true);

                if (Input.GetMouseButton(1))
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

    IEnumerator tilCanShoot()
    {
        float time = 0;

        while (time < 1 / bulletsPerSecond)
        {
            time += Time.deltaTime;
            tilNextShotLoading.fillAmount = time / (1 / bulletsPerSecond);
            yield return null;
        }
        tilNextShotLoading.fillAmount = 1;
        yield return new WaitForSeconds(0.1f);
        tilNextShotLoading.fillAmount = 0;

        canShoot = true;
    }
}
