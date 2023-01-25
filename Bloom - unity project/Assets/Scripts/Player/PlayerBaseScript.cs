using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inneh�ller alla variabler som flera av spelarens skripts kan beh�va
public class PlayerBaseScript : MonoBehaviour
{
    protected PlayerCameraScript cam;
    protected CollectibleCollectorScript cc;
    protected PlayerHealthScript h;
    protected PlayerMovementScript movement;
    protected ShootingScript shooting;

    protected Rigidbody rb;

    public virtual void Awake()
    {
        cam = FindObjectOfType<PlayerCameraScript>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CollectibleCollectorScript>();
        h = GetComponent<PlayerHealthScript>();
        movement = GetComponent<PlayerMovementScript>();
        shooting = GetComponent<ShootingScript>();
    }
}
