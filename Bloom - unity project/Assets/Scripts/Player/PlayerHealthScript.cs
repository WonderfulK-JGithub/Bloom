using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthScript : MonoBehaviour, IDamageable
{
    [SerializeField] int startHealth = 100;
    float _health;
    float health
    {
        get { return _health; }
        set { if(healthSlider != null) healthSlider.value = value / 100f; _health = value; }
    }

    public static bool isDead = false;

    [SerializeField] Slider healthSlider;

    [SerializeField] float damageOverlayFadeSpeed;

    [SerializeField] float damageTickTime;
    [SerializeField] int oilTickDamage;

    [SerializeField] float regenSpeed = 1;

    Image damageOverlay;
    Image damageOverlay2;

    ColorAdjustments colAd;

    float tickTimer;

    Coroutine[] fadeCoroutines = new Coroutine[3];

    PlayerCameraScript cam;

    private void Awake()
    {
        cam = FindObjectOfType<PlayerCameraScript>();

        Volume volume = FindObjectOfType<Volume>();
        ColorAdjustments tmp;

        if (volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            colAd = tmp;
        }

        health = startHealth;

        isDead = false;

        damageOverlay = GameObject.Find("DamageOverlay").GetComponent<Image>();
        damageOverlay2 = GameObject.Find("DamageOverlay (1)").GetComponent<Image>();
    }

    public void Damage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, 100);

        if (health <= 0) Die();

        if(health <= 40)
        {
            for (int i = 0; i < 3; i++)
            {
                if (fadeCoroutines[i] != null)
                    StopCoroutine(fadeCoroutines[i]);
            }
            fadeCoroutines[0] = StartCoroutine(FadeTo(damageOverlay, (((40f - health) / 40f) * 100f) / 256f));
            fadeCoroutines[1] = StartCoroutine(FadeTo(damageOverlay2, (((40f - health) / 40f) * 300f) / 256f));
            if (colAd != null)
                fadeCoroutines[2] = StartCoroutine(FadeSaturation(((40f - health) / 40f) * -100f));
        }
        else
        {

            for (int i = 0; i < 3; i++)
            {
                if (fadeCoroutines[i] != null)
                    StopCoroutine(fadeCoroutines[i]);
            }
            fadeCoroutines[0] = StartCoroutine(FadeTo(damageOverlay, 0));
            fadeCoroutines[1] = StartCoroutine(FadeTo(damageOverlay2, 0));
            if (colAd != null)
                fadeCoroutines[2] = StartCoroutine(FadeSaturation(0));
        }

        if(damage > 0)
        {
            cam.ShakeScreen();

            AudioManager.current.PlaySound(AudioManager.AudioNames.PlayerDamage);

            StopCoroutine(nameof(tilRegen));
            StopCoroutine(nameof(regen));
            if (health < 50)
            {
                StartCoroutine(nameof(tilRegen));
            }
        }   
    }

    IEnumerator FadeTo(Image img, float target)
    {
        while (Mathf.Abs(img.color.a - target) > 0.01f)
        {
            Color newColor = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(img.color.a, target, damageOverlayFadeSpeed * Time.deltaTime));
            img.color = newColor;
            yield return null;
        }
    }

    IEnumerator FadeSaturation(float target)
    {
        while (Mathf.Abs(colAd.saturation.value - target) > 0.01f)
        {
            if (colAd == null) break;
            colAd.saturation.value = Mathf.Lerp(colAd.saturation.value, target, damageOverlayFadeSpeed * Time.deltaTime);
            yield return null;
        }
    }


    public void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayerCameraScript.canLook = false;
        PlayerMovementScript.canMove = false;
        FindObjectOfType<PlayerCameraScript>().OnDeath();

        GameObject deathObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        deathObj.transform.position = transform.position + new Vector3(0, 1, 0);
        deathObj.AddComponent<Rigidbody>();
        Physics.IgnoreCollision(deathObj.GetComponent<CapsuleCollider>(), GetComponentInChildren<CapsuleCollider>());
        deathObj.GetComponent<MeshRenderer>().enabled = false;
        deathObj.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        GetComponentInChildren<CapsuleCollider>().enabled = false;
        gameObject.isStatic = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        FindObjectOfType<PlayerCameraScript>().deathObj = deathObj.transform;

        Invoke(nameof(Restart), 2f);
    }

    void Restart()
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Oil"))
        {
            tickTimer -= Time.deltaTime;
            if(tickTimer <= 0f)
            {
                Damage(oilTickDamage);
                tickTimer = damageTickTime;
                print("damage");
            }
        }
    }

    IEnumerator tilRegen()
    {
        yield return new WaitForSeconds(8);
        StartCoroutine(nameof(regen));
    }

    IEnumerator regen()
    {
        while(health < 50)
        {
            Damage(-regenSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

public interface IDamageable
{
    public void Damage(float damage);
    public void Die();
}

