using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthScript : MonoBehaviour, IDamageable
{
    [SerializeField] int startHealth = 100;
    int _health;
    int health
    {
        get { return _health; }
        set { if(healthSlider != null) healthSlider.value = value / 100f; _health = value; }
    }

    [SerializeField] Slider healthSlider; 

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

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl)) Damage(10);
        #endif
    }
}

public interface IDamageable
{
    public void Damage(int damage);
    public void Die();
}

