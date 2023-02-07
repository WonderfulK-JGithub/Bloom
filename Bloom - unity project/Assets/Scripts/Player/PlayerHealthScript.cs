using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class PlayerHealthScript : PlayerBaseScript, IDamageable
{
    //Health
    [SerializeField] int maxHealth = 100; //Det är gjort för att health ska vara procentuellt, med början på 100%.
    float _health;
    [HideInInspector] public float health
    {
        get { return _health; }
        set { if(healthSlider != null) healthSlider.value = value / maxHealth; healthSlider.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString("0") + "%"; _health = value; }
    }

    public static bool isDead = false;

    //UI
    [SerializeField] Slider healthSlider;

    //Regen
    [SerializeField] float regenSpeed = 1;
    bool isRegenerating = false;

    //damageoverlays
    [SerializeField] float damageOverlayFadeSpeed;
    Image damageOverlay;
    Image damageOverlay2;

    //Pretty shit solution, ngl
    Coroutine[] fadeCoroutines = new Coroutine[3];

    //Saturation
    ColorAdjustments colAd;
    float targetSaturation = 0;
    public static float saturationMultiplier = 1;

    //k-j
    [Header("Oil")]
    [SerializeField] float damageTickTime;
    [SerializeField] int oilTickDamage;
    float tickTimer;
    Image oilOverlay;
    [SerializeField] Material oilOverlayMat;
    float oilOverlayValue = 1;
    bool noOIl = false;

    public override void Awake()
    {
        base.Awake();

        oilOverlay = GameObject.Find("OilOverlay").GetComponent<Image>();
        oilOverlay.material = oilOverlayMat;
        oilOverlay.material.SetFloat("_idk", 1);

        saturationMultiplier = 1;

        Volume volume = FindObjectOfType<Volume>();
        ColorAdjustments tmp;

        if (volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            colAd = tmp;
        }

        health = maxHealth;

        isDead = false;

        damageOverlay = GameObject.Find("DamageOverlay").GetComponent<Image>();
        damageOverlay2 = GameObject.Find("DamageOverlay (1)").GetComponent<Image>();
    }

    public void Damage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0) Die();

        saturationMultiplier = 1;
        if (health <= 40)
        {
            for (int i = 0; i < 3; i++)
            {
                if (fadeCoroutines[i] != null)
                    StopCoroutine(fadeCoroutines[i]);
            }
            fadeCoroutines[0] = StartCoroutine(FadeTo(damageOverlay, (((40f - health) / 40f) * 100f) / 256f));
            fadeCoroutines[1] = StartCoroutine(FadeTo(damageOverlay2, (((40f - health) / 40f) * 300f) / 256f));

            saturationMultiplier = 1f - ((40f - health) / 40f);
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
        }

        if(damage > 0)
        {
            cam.ShakeScreen();

            AudioManager.current.PlaySound(AudioManager.AudioNames.PlayerDamage);

            StopCoroutine(nameof(tilNoOil));
            StartCoroutine(nameof(tilNoOil));

            StopCoroutine(nameof(tilRegen));
            isRegenerating = false;
            //StopCoroutine(nameof(regen));
            if (health < maxHealth / 2)
            {
                StartCoroutine(nameof(tilRegen));
            }
        }   
        else if(damage < 0)
        {
            noOIl = true;
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FindObjectOfType<SceneTransition>().EnterScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        Regen();
        GetRidOfOil();

        if (transform.position.y < -20) Die();

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl)) Damage(10);
        #endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oil"))
        {
            tickTimer = 0f;
        }
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
                oilOverlayValue -= 0.1f;
                oilOverlay.material.SetFloat("_idk", oilOverlayValue);
            }
        }
    }

    IEnumerator tilRegen()
    {
        yield return new WaitForSeconds(5);
        isRegenerating = true;
        //StartCoroutine(nameof(regen));
    }

    void Regen()
    {
        if(isRegenerating && PlayerCameraScript.inRadiusOfPlant)
        {
            if (health < (maxHealth / 2))
            {
                Damage(-regenSpeed * Time.deltaTime);
            }
            else
            {
                isRegenerating = false;
            }           
        }
    }

    IEnumerator tilNoOil()
    {
        yield return new WaitForSeconds(3);
        noOIl = true;
    }

    void GetRidOfOil()
    {
        if (!noOIl || !PlayerCameraScript.inRadiusOfPlant) return;

        oilOverlayValue = 1;
        oilOverlay.material.SetFloat("_idk", oilOverlayValue);
        noOIl = false;
    }

    public void SetMaxHealth(int newMaxHealth) //För upgrades
    {
        maxHealth = newMaxHealth;
        health = health;
    }
}

public interface IDamageable
{
    public void Damage(float damage);
    public void Die();
}

