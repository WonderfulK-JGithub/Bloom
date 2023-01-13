using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] GameObject bullet;

    [SerializeField] float bulletScaleSpeed;
    [SerializeField] float bulletVelSpeed;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = Instantiate(bullet, barrel.position, barrel.rotation);
            newBullet.GetComponent<WaterBullet>().SetVelocity(barrel.forward, 1);
            Physics.IgnoreCollision(newBullet.GetComponent<SphereCollider>(), GetComponentInChildren<CapsuleCollider>());

            newBullet.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            newBullet.GetComponentInChildren<ParticleSystem>().startSize = 0.0008f;
            StartCoroutine(scaleBullet(newBullet.transform));
            StartCoroutine(speedUpBullet(newBullet.GetComponent<WaterBullet>()));
        }
    }

    IEnumerator scaleBullet(Transform tr)
    {
        ParticleSystem ps = tr.GetComponentInChildren<ParticleSystem>();

        while(tr.localScale.x < 0.0999f)
        {
            if (tr == null) break;

            Vector3 newSize = Vector3.Lerp(tr.localScale, new Vector3(0.1f, 0.1f, 0.1f), bulletScaleSpeed * Time.deltaTime);
            tr.localScale = newSize;
            ps.startSize = newSize.x;
            yield return null;
        }
    }

    IEnumerator speedUpBullet(WaterBullet bul)
    {
        float v = 1;
        while (v < 24.999f)
        {
            if (bul == null) break;

            v = Mathf.Lerp(v, 25f , bulletVelSpeed * Time.deltaTime);
            bul.SetVelocity(bul.transform.forward, v);
            Debug.Log(v);
            yield return null;
        }
    }
}
