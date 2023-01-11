using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    [SerializeField] float sensitivity = 100f;

    [SerializeField] Camera cam;

    float xRot, yRot;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        yRot += mouseX;

        cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        rb.MoveRotation(Quaternion.Euler(0,yRot,0));
    }
}
