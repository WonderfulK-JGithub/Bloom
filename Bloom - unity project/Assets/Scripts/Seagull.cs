using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour,IWaterable
{
    [Header("General")]
    [SerializeField] float rotationSpeed;
    [SerializeField] int healthPoints;

    [Header("Idle")]
    [SerializeField] float walkSpeed;
    [SerializeField] float idleRadius;
    [SerializeField] float timeBetweenIdleWalk;
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

    SeagullState state;

    Rigidbody rb;
    Collider col;

    Vector3 targetPos;
    Vector3 centerPos;

    Vector3 currentThing;
    Vector3 targetForward;

    float timer;
    float rotationAroundPlayer;
    float diveStartY;
    float oilTimer;

    Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        centerPos = transform.position;
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
                            if(_hit.point.y == transform.position.y - 0.5f)
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
                }

                oilTimer += Time.deltaTime;

                if(oilTimer >= timeBetweenOilPoop)
                {
                    DropOil();
                    oilTimer = 0f;
                }

                break;
            
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

                rb.velocity = walkSpeed * (targetPos - transform.position).normalized;

                targetForward = rb.velocity.normalized;

                if (Vector3.Distance(transform.position,targetPos) < walkSpeed * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector3.zero;
                    transform.position = targetPos;
                }



                Collider[] _player = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
                if(_player.Length > 0)
                {
                    player = _player[0].transform;

                    state = SeagullState.StartCircle;
                    rb.useGravity = false;
                    col.isTrigger = true;

                    Vector2 _distanceDir = new Vector2(player.position.x - transform.position.x, player.position.z - transform.position.z).normalized;
                    rotationAroundPlayer = Vector2.Angle(Vector2.up, _distanceDir) - 90f;
                    
                    if (transform.position.x < player.position.x) rotationAroundPlayer += 180;
                    print(rotationAroundPlayer);

                    currentThing = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(player.position.x, 0f, player.position.z);
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

                float _targetY = player.position.y + yOffset;

                float _percent = Mathf.InverseLerp(diveStartY, _targetY, transform.position.y);

                float _relativeDiveSpeed = diveCurve.Evaluate(_percent);
                float _relativeDiveSpeedX = diveCurveX.Evaluate(_percent);
                _yPos = Mathf.MoveTowards(transform.position.y, _targetY, diveSpeed * _relativeDiveSpeed * Time.deltaTime);

                transform.position = new Vector3(transform.position.x, _yPos, transform.position.z);

                Vector3 _dirToPlayer = (new Vector3(player.position.x, 0f, player.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
                rb.velocity = _dirToPlayer * horizontalSpeed * _relativeDiveSpeedX;

                targetForward = (rb.velocity + diveSpeed * _relativeDiveSpeed * Vector3.down).normalized;

                break;
        }

        float _extraSpeed = state == SeagullState.FlyUp ? 2f : 1f;

        transform.forward = Vector3.MoveTowards(transform.forward,targetForward,rotationSpeed * Time.deltaTime * _extraSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if(state == SeagullState.Dive)
            {
                state = SeagullState.StartCircle;
                rotationAroundPlayer -= 180f;
                currentThing = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(player.position.x, 0f, player.position.z);
                
            }
        }
    }

    public void Water()
    {
        healthPoints--;

        if(healthPoints == 0)
        {

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
}