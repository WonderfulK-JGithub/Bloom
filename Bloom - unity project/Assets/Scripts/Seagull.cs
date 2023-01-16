using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour
{
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
    [SerializeField] float yOffset;

    SeagullState state;

    Rigidbody rb;

    Vector3 targetPos;
    Vector3 centerPos;

    Vector3 currentThing;

    float timer;
    float rotationAroundPlayer;
    float diveStartY;

    Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
                rb.useGravity = true;

                rb.velocity = walkSpeed * (targetPos - transform.position).normalized;

                if(Vector3.Distance(transform.position,targetPos) < walkSpeed * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector3.zero;
                    transform.position = targetPos;
                }

                Collider[] _player = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
                if(_player.Length > 0)
                {
                    player = _player[0].transform;

                    state = SeagullState.StartCircle;

                    Vector2 _distanceDir = new Vector2(player.position.x - transform.position.x, player.position.z - transform.position.z).normalized;
                    rotationAroundPlayer = Vector2.Angle(Vector2.up, _distanceDir);
                    
                    if (transform.position.x < player.position.x) rotationAroundPlayer += 180;
                    print(rotationAroundPlayer);

                    currentThing = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(player.position.x, 0f, player.position.z);
                }
                #endregion
                break;
            case SeagullState.StartCircle:
                #region
                rb.useGravity = false;

                _radiusOffset = playerCircleRadius * new Vector3(Mathf.Cos(rotationAroundPlayer * Mathf.Deg2Rad), 0f, Mathf.Sin(rotationAroundPlayer * Mathf.Deg2Rad)).normalized;

                currentThing = Vector3.MoveTowards(currentThing, _radiusOffset, horizontalSpeed * Time.fixedDeltaTime);

                _yPos = Mathf.MoveTowards(transform.position.y, player.position.y + heightOverPlayer, verticalSpeed * Time.fixedDeltaTime);

                transform.position = new Vector3(player.position.x + currentThing.x, _yPos, player.position.z + currentThing.z);

                
                if (currentThing == _radiusOffset)
                {
                    state = SeagullState.FlyUp;
                    timer = 0f;
                }

                break;
            #endregion
            case SeagullState.FlyUp:
                #region
                rb.useGravity = false;

                rotationAroundPlayer += playerCircleSpeed * Time.fixedDeltaTime;
                

                _radiusOffset = playerCircleRadius * new Vector3(Mathf.Cos(rotationAroundPlayer * Mathf.Deg2Rad), 0f, Mathf.Sin(rotationAroundPlayer * Mathf.Deg2Rad)).normalized;

                currentThing = Vector3.MoveTowards(currentThing, _radiusOffset, 123f);
                

                _yPos = Mathf.MoveTowards(transform.position.y, player.position.y + heightOverPlayer, verticalSpeed * Time.fixedDeltaTime);

                transform.position = new Vector3(player.position.x + currentThing.x, _yPos, player.position.z + currentThing.z);

                #endregion
                break;
            case SeagullState.Dive:

                float _targetY = player.position.y + yOffset;

                float _relativeDiveSpeed = diveCurve.Evaluate(Mathf.InverseLerp(diveStartY, _targetY, transform.position.y));
                _yPos = Mathf.MoveTowards(transform.position.y, _targetY, diveSpeed * _relativeDiveSpeed * Time.deltaTime);

                transform.position = new Vector3(transform.position.x, _yPos, transform.position.z);
                

                break;
        }
    }
}

public enum SeagullState
{
    Idle,
    StartCircle,
    FlyUp,
    Dive,
}