using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float followCameraSpeed;
    [SerializeField] float velocityMultiplier;
    [SerializeField] float velocitySmooth;

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    [SerializeField] float maxSway;

    [Header("Weapon Bob Settings")]
    [SerializeField] bool weaponBob = true;
    [SerializeField] Vector2 weaponBobScale;
    [SerializeField] Vector2 weaponBobSpeed;
    [SerializeField] float weaponBobSmooth;
    Vector3 actualWeaponBobOffset = Vector3.zero;

    [Header("Recoil Settings")]
    [SerializeField] float recoilScale;
    float recoilOffset;
    [SerializeField]float recoilSpeed;

    [Header("References")]
    [SerializeField] Transform gun;
    [SerializeField] Camera cam;
    [SerializeField] Camera weaponCam;
    Rigidbody playerRb;

    Vector3 offset;


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (PlayerCameraScript.canLook)
        {
            gun.localPosition = cam.transform.localPosition;
        }
        else
        {
            gun.localPosition = weaponCam.transform.localPosition;
        }

        offset = Vector3.Lerp(offset, playerRb.velocity * velocityMultiplier, velocitySmooth * Time.deltaTime);
        gun.position -= offset;

        WeaponBob();
        gun.localPosition += actualWeaponBobOffset;

        gun.localRotation = Quaternion.Euler(cam.transform.eulerAngles.x, 0f, 0f);

        gun.position += gun.forward * -recoilOffset;
        recoilOffset = Mathf.Lerp(recoilOffset, 0, recoilSpeed * Time.deltaTime);
    }

    private void Update()
    {
        if (!PlayerCameraScript.canLook) return;

        //gun.localRotation = Quaternion.Lerp(gun.localRotation, Quaternion.Euler(cam.transform.eulerAngles.x, 0f, 0f), followCameraSpeed * Time.deltaTime);

        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(Mathf.Clamp(-mouseY, -maxSway, maxSway), Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(Mathf.Clamp(mouseX, -maxSway, maxSway), Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;
        //targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, Mathf.Clamp(targetRotation.eulerAngles.y, -maxSway, maxSway), targetRotation.eulerAngles.z);

        gun.GetChild(0).localRotation = Quaternion.Slerp(gun.GetChild(0).localRotation, targetRotation, smooth * Time.deltaTime);
    }

    void WeaponBob()
    {
        if (!weaponBob || PlayerHealthScript.isDead) return;

        Vector3 weaponBobOffset = Vector3.zero;
        if (PlayerMovementScript.isMoving)
        {
            weaponBobOffset.x = Mathf.Sin(Time.time * weaponBobSpeed.x) * weaponBobScale.x;
            weaponBobOffset.y = Mathf.Sin(Time.time * weaponBobSpeed.y) * weaponBobScale.y;
        }

        //cam.transform.localPosition = new Vector3(0, 1.75f) + weaponBobOffset;
        actualWeaponBobOffset = Vector3.Lerp(actualWeaponBobOffset, weaponBobOffset, weaponBobSmooth * Time.deltaTime);
    }

    public void Fire()
    {
        recoilOffset = recoilScale;
    }
}
