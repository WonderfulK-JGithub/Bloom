using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCannon : MonoBehaviour
{
    [SerializeField] float spawnRate;
    [SerializeField] GameObject bulletPrefab;

    float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnRate)
        {
            timer = 0f;
            Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<WaterBullet>().SetVelocity(transform.forward, 15f);
        }
    }
}
