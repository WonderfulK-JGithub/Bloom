using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthScript : MonoBehaviour, IDamageable
{
    [SerializeField] int startHealth = 100;
    int health;

    private void Awake()
    {
        health = startHealth;
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0) Die();
    }

    public void Die()
    {
        //Temporärt, gör nåt bättre - Max
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (transform.position.y < -20) Die();

        if (Input.GetKeyDown(KeyCode.LeftControl)) Damage(10);
    }
}

public interface IDamageable
{
    public void Damage(int damage);
    public void Die();
}

