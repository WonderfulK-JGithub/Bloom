using System;
using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour, IWaterable
{
    protected Transform target;
    public float rotationSpeed = 18;
    public float moveSpeed = 4.5f;
    protected Rigidbody rb;
    protected bool chase = true;
    protected bool onGround = false;
    public float detectionRange = 10;
    protected Coroutine wander;
    [HideInInspector] public bool brake = false;
    [SerializeField] LayerMask ignoreLayers;
    public float hp = 100;
    protected float distanceToPlayer;
    protected bool lastchase = true;
    protected bool hasTransformed = false;
    public SkinnedMeshRenderer color;
    [Tooltip("Oljans partikelsystem!")] public ParticleSystem ps;
    public GameObject utropstecken;
    public ParticleSystem friendlyPS;
    bool fpshp = false;
    

    protected virtual void Start()
    {
        Time.timeScale = 1;
        target = FindObjectOfType<PlayerHealthScript>().transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    protected virtual void Update()
    {
        lastchase = chase;

        chase = false;
        distanceToPlayer = Vector3.Distance(transform.position + (transform.up * transform.lossyScale.y / 2), target.position);

        RaycastHit hit;
        Debug.DrawRay(transform.position  + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position));
        if (distanceToPlayer < detectionRange)
        {
            int mask = (1 << 9);
            mask += (1 << 4);
            mask = ~mask;

            //??? wtf <<<
            if (Physics.Raycast(transform.position + (transform.up * transform.lossyScale.y / 2), (target.position - transform.position), out hit, detectionRange, ~ignoreLayers) && !PlayerMovementScript.isBathing)
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    chase = true;
                }
            }
        }

        if (hp <= 0)
        {
            StartCoroutine(Transformation());
        }

        Movement();
        Rotation();
    }

    private void FixedUpdate()
    {
        rb.velocity += Gravity() * Time.fixedDeltaTime;
    }

    protected virtual void Movement()
    {

        if (chase)
        {
            if (!lastchase)
            {
                utropstecken.SetActive(true);
                StopCoroutine(wander);
            }

            rb.velocity = (transform.forward * moveSpeed) + Gravity() * Convert.ToInt32(!onGround);
        }
        else
        {
            if (lastchase)
            {
                wander = StartCoroutine(Wander());
            }
        }
    }

    protected virtual void Rotation()
    {
        if (chase)
        {
            rb.angularVelocity = Vector3.zero;
            Quaternion fullRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), Time.deltaTime * rotationSpeed);
            rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, fullRot.eulerAngles.y, rb.rotation.z));
        }
    }

    protected virtual IEnumerator Transformation()
    {
        chase = false;
        if (!hasTransformed)
        {
            hasTransformed = true;
            detectionRange = 0;

            float t = 0;
            Vector3 scale = transform.localScale;
            Vector3 targetScale = transform.localScale / 1.5f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(scale, targetScale, t);

                t += Time.deltaTime;
                yield return 0;
            }
        }
        

    }
    protected virtual Vector3 Gravity() { return Physics.gravity; }

    public void Water()
    {
        if (detectionRange > 0)
        {
            ps.Play();
        }
        DamageEnemy(10);
    }
    public void DamageEnemy(float damage)
    {
        hp -= damage;
        foreach (var mat in color.materials)
        {
            if (hp >= 0 && mat.HasProperty("_OilLevel"))
            {
                mat.SetFloat("_OilLevel", hp / 100f);
            }
            else
            {
                mat.SetFloat("_OilLevel", 0);
            }
        }

        if (hp <= 0 && !fpshp)
        {
            friendlyPS.Play();
            fpshp = true;
        }
        StartCoroutine(DamageTint());
    }

    IEnumerator DamageTint()
    {
        Color[] ogcols = new Color[color.materials.Length];

        for (int i = 0; i < color.materials.Length; i++)
        {
            if (color.materials[i].HasProperty("_Tint"))
            {
                ogcols[i] = color.materials[i].GetColor("_Tint");
            }
        }
        Color newColor = new Color(0.5f, 0.75f, 1, 1);

        foreach (var mat in color.materials)
        {
            if (mat.HasProperty("_Tint"))
            {
                mat.SetColor("_Tint", newColor);
            }
        }

        float t = 1;

        while (t > 0)
        {
            for (int i = 0; i < color.materials.Length; i++)
            {
                if (color.materials[i].HasProperty("_Tint"))
                {
                    color.materials[i].SetColor("_Tint", Color.Lerp(newColor, ogcols[i], t));
                }
            }
            t -= Time.deltaTime * 3;
            yield return null;
        }
        for (int i = 0; i < color.materials.Length; i++)
        {
            if (color.materials[i].HasProperty("_Tint"))
            {
                color.materials[i].SetColor("_Tint", ogcols[i]);
            }
        }
    }
    protected virtual IEnumerator Wander()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float t = 0;
            float randomRotation = UnityEngine.Random.Range(0f, 360f);

            if (Mathf.Abs(transform.position.x - startPos.x) > 10 || Mathf.Abs(transform.position.z - startPos.z) > 10)
            {
                randomRotation = Quaternion.LookRotation(startPos).y + 180;
            }
            while (t < 1)
            {
                rb.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, randomRotation, 0)), t); 
                t += Time.deltaTime;
                yield return 0;
            }

            float secs = UnityEngine.Random.Range(1f, 3f);
            float time = 0;
            while (time < secs && !brake)
            {
                rb.velocity = (transform.forward * moveSpeed) + Gravity() * Convert.ToInt32(!onGround);
                rb.angularVelocity = new Vector3(0, 0, 0);
                time += Time.deltaTime;
                yield return 0;
            }

            rb.velocity = Gravity();
        }
    }

    public void DamagePlayer(int avgDamage)
    {
        if (target.GetComponent<PlayerHealthScript>())
        {
            target.GetComponent<PlayerHealthScript>().Damage(UnityEngine.Random.Range(avgDamage - 5, avgDamage + 6));
        }
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && detectionRange > 0)
        {
            DamagePlayer(10);
        }
    }
    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onGround = true;
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onGround = false;
        }
    }
}
