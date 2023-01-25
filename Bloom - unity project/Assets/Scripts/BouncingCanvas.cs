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
        startY = transform.position.y;
        currentY = startY;

        speed = startForce;

        bounce = true;

        nextForce = startForce;

        timer = gravityTime;

        if (appearTime > 0f)
        {
            CancelInvoke();
            Invoke("Disapear", appearTime);
        }
    }

    
    private void FixedUpdate()
    {
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
                transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
            }

            
        }

        transform.LookAt(cameraTransform, Vector3.up);
    }

    public void Disapear()
    {
        transform.parent.gameObject.SetActive(false);
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
