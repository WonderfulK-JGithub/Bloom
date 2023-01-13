using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float sensitivity = 100f;

    [Header("Headbobbing")]
    [SerializeField] bool headBob = true;
    [SerializeField] Vector2 headBobScale;
    [SerializeField] Vector2 headBobSpeed;
    [SerializeField] float headBobReturnSpeed;
    Vector3 headBobOffset = Vector3.zero;

    [Header("References")]
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

        HeadBob();
    }

    void HeadBob()
    {
        if (!headBob) return;

        if (PlayerMovementScript.isMoving)
        {
            headBobOffset.x = Mathf.Sin(Time.time * headBobSpeed.x) * headBobScale.x;
            headBobOffset.y = Mathf.Sin(Time.time * headBobSpeed.y) * headBobScale.y;
        }
        else
        {
            headBobOffset = Vector3.Lerp(headBobOffset, Vector3.zero, headBobReturnSpeed * Time.deltaTime);
            //headBobOffset = Vector3.zero;
        }

        cam.transform.localPosition = new Vector3(0, 1.75f) + headBobOffset;
        //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 1.75f) + headBobOffset, headBobReturnSpeed * Time.deltaTime);
    }
}
