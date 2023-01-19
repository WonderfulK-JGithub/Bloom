using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour,IWaterable
{
    [Header("General")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float rotationSpeed2;
    [SerializeField] int startHealthPoints;
    [SerializeField] int damage;
    [SerializeField] Animator anim;
    [SerializeField] Gradient hitFlashGradient;
    [SerializeField] float hitFlashDuration;

    [Header("Idle")]
    [SerializeField] float walkSpeed;
    [SerializeField] float idleRadius;
    [SerializeField] float timeBetweenIdleWalk;
    [SerializeField] float heighestYDiff;
    [SerializeField] LayerMask groundLayer;

    [Header("Detect Player")]
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask playerLayer;

    [Header("Flying")]
    [SerializeField] float horizontalSpeed;
    [SerializeField] float verticalSpeed;
    [SerializeField] float heightOverPlayer;
    [SerializeField] float playerCircleRadius;
    [SerializeField] float playerCircleSpeed;
    [SerializeField] float timeBetweenDives;

    [Header("Dive")]
    [SerializeField] float diveSpeed;
    [SerializeField] AnimationCurve diveCurve;
    [SerializeField] AnimationCurve diveCurveX;
    [SerializeField] float yOffset;

    [Header("DropOil")]
    [SerializeField] float timeBetweenOilPoop;
    [SerializeField] GameObject oilDropPrefab;
    [SerializeField] float dropStartSpeed;

    [Header("Happy")]
    [SerializeField] float flappForce;
    [SerializeField] float flappingSpeed;
    [SerializeField] float flappingDistanceY;

    SeagullState state;

    Rigidbody rb;
    Collider col;
    [SerializeField] SkinnedMeshRenderer rend;

    Vector3 targetPos;
    Vector3 centerPos;

    Vector3 currentThing;
    Vector3 targetForward;

    int healthPoints;

    float timer;
    float rotationAroundPlayer;
    float diveStartY;
    float oilTimer;
    float hitFlashTimer;

    Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        //rend = GetComponent<Renderer>();

        centerPos = transform.position;

        healthPoints = startHealthPoints;
    }

    private void Update()
    {
        switch (state)
        {
            case SeagullState.Idle:

                timer += Time.deltaTime;

                if(timer >= timeBetweenIdleWalk)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector3 _dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                        Vector3 _newTarget = _dir * Random.Range(0f, idleRadius) + centerPos;

                        if (Physics.Raycast(_newTarget + Vector3.up, Vector3.down, out RaycastHit _hit, groundLayer))
                        {
                            if(Mathf.Abs(_hit.point.y - transform.position.y - 0.5f) > heighestYDiff)
                            {
                                targetPos = _newTarget;
                                timer = 0f;
                                break;
                            }
                        }
                    }
                    
                }

                break;
            case SeagullState.FlyUp:

                timer += Time.deltaTime;

                if (timer >= timeBetweenDives)
                {
                    state = SeagullState.Dive;
                    diveStartY = transform.position.y;
                    anim.Play("Bird_Dive");
                }

                oilTimer += Time.deltaTime;

                if(oilTimer >= timeBetweenOilPoop)
                {
                    DropOil();
                    oilTimer = 0f;
                }

                break;
            case SeagullState.Happy:

                if (transform.position.y < centerPos.y)
                {
                    rb.velocity = new Vector3(0f, flappForce, 0f);
                }

                break;
        }

        if(hitFlashTimer > 0f)
        {
            hitFlashTimer -= Time.deltaTime;

            Color _color = hitFlashGradient.Evaluate(1f - hitFlashTimer / hitFlashDuration);
            foreach (var material in rend.materials)
            {
                material.SetColor("_OtherTint", _color);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 _radiusOffset;
        float _yPos;

        switch (state)
        {
            case SeagullState.Idle:
                #region

                Vector3 _vel = walkSpeed * (targetPos - transform.position).normalized;

                rb.velocity = new Vector3(_vel.x, rb.velocity.y, _vel.z);

                Vector3 _dir = rb.velocity.normalized;

                if(rb.velocity != Vector3.zero)
                {
                    float _angle = Vector2.Angle(new Vector2(_dir.x, _dir.z), Vector2.up);
                    _angle *= Mathf.Sign(_dir.x);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, _angle, 0f), rotationSpeed2 * Time.fixedDeltaTime);

                }

                if (Vector3.Distance(transform.position,targetPos) < walkSpeed * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector3.zero;
                    transform.position = targetPos;
                }



                Collider[] _player = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
                if(_player.Length > 0)
                {
                    StartTargeting(_player[0].transform);
                }
                #endregion
                break;
            case SeagullState.StartCircle:
                #region

                _radiusOffset = playerCircleRadius * new Vector3(Mathf.Cos(rotationAroundPlayer * Mathf.Deg2Rad), 0f, Mathf.Sin(rotationAroundPlayer * Mathf.Deg2Rad)).normalized;

                currentThing = Vector3.MoveTowards(currentThing, _radiusOffset, horizontalSpeed * Time.fixedDeltaTime);

                _yPos = Mathf.MoveTowards(transform.position.y, player.position.y + heightOverPlayer, verticalSpeed * Time.fixedDeltaTime);

                transform.position = new Vector3(player.position.x + currentThing.x, _yPos, player.position.z + currentThing.z);

                targetForward = (_radiusOffset - currentThing).normalized;
                if (currentThing == _radiusOffset)
                {
                    state = SeagullState.FlyUp;
                    timer = 0f;


                }

                break;
            #endregion
            case SeagullState.FlyUp:
                #region

                rotationAroundPlayer += playerCircleSpeed * Time.fixedDeltaTime;
                

                _radiusOffset = playerCircleRadius * new Vector3(Mathf.Cos(rotationAroundPlayer * Mathf.Deg2Rad), 0f, Mathf.Sin(rotationAroundPlayer * Mathf.Deg2Rad)).normalized;

                currentThing = Vector3.MoveTowards(currentThing, _radiusOffset, 123f);
                

                _yPos = Mathf.MoveTowards(transform.position.y, player.position.y + heightOverPlayer, verticalSpeed * Time.fixedDeltaTime);

                transform.position = new Vector3(player.position.x + currentThing.x, _yPos, player.position.z + currentThing.z);
                

                Vector2 _circularDir = Vector2.Perpendicular(new Vector2(player.position.x, player.position.z) - new Vector2(transform.position.x, transform.position.z)).normalized;

                targetForward = new Vector3(_circularDir.x,0f,_circularDir.y) * -1f;
                #endregion
                break;
            case SeagullState.Dive:
                #region
                float _targetY = player.position.y + yOffset;

                float _percent = Mathf.InverseLerp(diveStartY, _targetY, transform.position.y);

                float _relativeDiveSpeed = diveCurve.Evaluate(_percent);
                float _relativeDiveSpeedX = diveCurveX.Evaluate(_percent);
                _yPos = Mathf.MoveTowards(transform.position.y, _targetY, diveSpeed * _relativeDiveSpeed * Time.deltaTime);

                transform.position = new Vector3(transform.position.x, _yPos, transform.position.z);

                Vector3 _dirToPlayer = (new Vector3(player.position.x, 0f, player.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
                rb.velocity = _dirToPlayer * horizontalSpeed * _relativeDiveSpeedX;

                targetForward = (rb.velocity + diveSpeed * _relativeDiveSpeed * Vector3.down).normalized;
                #endregion
                break;
            
        }

        float _extraSpeed = state == SeagullState.FlyUp ? 2f : 1f;

        if(state != SeagullState.Idle && state != SeagullState.Happy) transform.forward = Vector3.MoveTowards(transform.forward,targetForward,rotationSpeed * Time.deltaTime * _extraSpeed);
    }

    void StartTargeting(Transform _player)
    {
        player = _player;

        state = SeagullState.StartCircle;
        rb.useGravity = false;
        col.isTrigger = true;

        Vector2 _distanceDir = new Vector2(player.position.x - transform.position.x, player.position.z - transform.position.z).normalized;
        rotationAroundPlayer = Vector2.Angle(Vector2.up, _distanceDir) - 90f;

        if (transform.position.x < player.position.x) rotationAroundPlayer += 180;
        print(rotationAroundPlayer);

        currentThing = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(player.position.x, 0f, player.position.z);

        anim.Play("Bird_Fly");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && state != SeagullState.Happy)
        {
            if(state == SeagullState.Dive)
            {
                state = SeagullState.StartCircle;
                rotationAroundPlayer -= 180f;
                currentThing = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(player.position.x, 0f, player.position.z);
                anim.Play("Bird_Fly");
            }
            other.GetComponentInParent<PlayerHealthScript>().Damage(damage);
            print(other.GetComponentInParent<PlayerHealthScript>().name);
        }
    }

    public void Water()
    {
        healthPoints--;

        hitFlashTimer = hitFlashDuration;

        if(healthPoints == 0)
        {
            Happy();
            foreach (var _material in rend.materials)
            {
                _material.SetFloat("_OilLevel", 1f);
            }
        }
        else
        {
            if (state == SeagullState.Idle) StartTargeting(FindObjectOfType<PlayerHealthScript>().transform);
            //rend.material.SetFloat("_OilLevel", 1f - healthPoints / (float)startHealthPoints);
            foreach (var _material in rend.materials)
            {
                _material.SetFloat("_OilLevel", 1f - healthPoints / (float)startHealthPoints);
            }
        }
    }

    void Happy()
    {
        state = SeagullState.Happy;
        
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        anim.speed = flappingSpeed;
        anim.Play("Bird_Fly");

        if(Physics.Raycast(transform.position, Vector3.down,out RaycastHit _hit, groundLayer))
        {
            centerPos = _hit.point + Vector3.up * flappingDistanceY;
        }
        else
        {
            centerPos = transform.position;
        }

        
    }



    public void DropOil()
    {
        OilBullet _oil = Instantiate(oilDropPrefab, transform.position, Quaternion.identity).GetComponent<OilBullet>();

        _oil.SetVelocity(Vector3.down, dropStartSpeed);
    }
}

public enum SeagullState
{
    Idle,
    StartCircle,
    FlyUp,
    Dive,
    Happy,
}