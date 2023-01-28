using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingCanvas : MonoBehaviour
{
    [SerializeField] float startForce;
    [SerializeField] float bounceFallOff;
    [SerializeField] bool flatFallOff = true;
    [SerializeField] bool forceOnEnable = true;
    [SerializeField] float gravity;
    [SerializeField] float gravityModifier;
    [SerializeField] float gravityTime;
    [SerializeField] float appearTime;

    Transform cameraTransform;

    float startY;
    float currentY;

    float speed;
    float nextForce;

    float timer;

    bool bounce;
    bool active;

    float appearTimer;

    private void Awake()
    {
        if (forceOnEnable)
        {
            StartBounce();
        }

        cameraTransform = Camera.main.transform;
    }

    
    public void StartBounce()
    {
        startY = transform.localPosition.y;
        currentY = startY;

        speed = startForce;

        bounce = true;
        active = true;

        nextForce = startForce;

        timer = gravityTime;

        appearTimer = appearTime;    }

    private void Update()
    {
        if (PauseMenu.paused || !active) return;

        appearTimer -= Time.deltaTime;

        

        if (appearTimer <= 0f)
        {
            transform.parent.gameObject.SetActive(false);
            active = false;
        }
    }

    private void FixedUpdate()
    {
        if (PauseMenu.paused) return;

        if (bounce)
        {
            currentY += speed;

            if(timer >= 0f)
            {
                speed -= gravity * Time.fixedDeltaTime;
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                speed -= gravity * Time.fixedDeltaTime * gravityModifier;   
            }
            

            if(currentY <= startY)
            {
                FallOffBounce();
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, currentY, transform.localPosition.z);
            }

            
        }

        transform.LookAt(cameraTransform, Vector3.up);
    }

    void FallOffBounce()
    {
        if (flatFallOff)
        {
            nextForce -= bounceFallOff;
            if (nextForce < 0f) nextForce = 0f;
        }
        else
        {
            nextForce *= bounceFallOff;
        }

        

        speed = nextForce;
        currentY = startY;

        

        if (nextForce < 0f || nextForce < startForce / 20f)
        {
            bounce = false;
        }

        timer = gravityTime * (nextForce / startForce);
    }

    private void OnEnable()
    {
        if (forceOnEnable)
        {
            StartBounce();
        }
    }

    
}
