using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [Header("Screenshake")]
    [SerializeField] float screenShakeTime;
    [SerializeField] float screenShakeMagnitude;

    [Header("References")]
    [SerializeField] Camera cam;

    float xRot, yRot;

    Rigidbody rb;

    public static bool canLook = true;
    public Transform deathObj;

    float shakeTimer;
    float shakePower;
    float powerReduction;

    public static bool cameraShake = true;

    ColorAdjustments colAd;
    float targetSaturation = 0;
    float saturationOffset = 0;
    [Header("Saturation")]
    [SerializeField] float saturationSpeed;

    List<Plant> plants = new List<Plant>();

    private void Awake()
    {
        plants.AddRange(FindObjectsOfType<Plant>());

        cameraShake = System.Convert.ToBoolean(PlayerPrefs.GetInt("CameraShakeBool", 1));

        Volume volume = FindObjectOfType<Volume>();
        ColorAdjustments tmp;

        if (volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            colAd = tmp;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canLook = true;

        deathObj = transform;
    }

    void Update()
    {
        bool b = false;
        foreach (Plant pla in plants)
        {
            if (Vector3.Distance(transform.position, pla.transform.position) < 10)
            {
                b = true;
                break;
            }
        }

        if (b) targetSaturation = 50;

        if (!canLook) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        yRot += mouseX;

        cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        rb.MoveRotation(Quaternion.Euler(0,yRot,0));

        HeadBob();

        ScreenShake();

        if (colAd == null) return;
        colAd.saturation.value = Mathf.Lerp(colAd.saturation.value, targetSaturation + saturationOffset, saturationSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (!canLook)
        {
            cam.transform.position = deathObj.position + deathObj.up * 0.75f;
        }
    }

    public void OnDeath()
    {
        cam.transform.parent = null;
        cam.transform.GetChild(0).parent = transform;
    }

    void HeadBob()
    {
        if (!headBob || !cameraShake) return;

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

    void ScreenShake()
    {
        if (!cameraShake) return;

        if (shakeTimer > 0f)//screneshake (samma metod som i Juicyness)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 shakePos = new Vector3();
            shakePos.x = Random.Range(-shakePower, shakePower);
            shakePos.y = Random.Range(-shakePower, shakePower);

            //Vector3 truePos = shakePos.x * weap.right + shakePos.y * transform.up;//screenshaken tar hänsyn till kameravinkeln

            cam.transform.localPosition += shakePos;

            shakePower -= powerReduction * Time.deltaTime;
        }
    }

    public void ShakeScreen()
    {
        shakeTimer = screenShakeTime;
        shakePower = screenShakeMagnitude;
        powerReduction = shakePower / shakeTimer;
    }

    public void SetSaturationOFfset(float sat)
    {
        saturationOffset = sat;
    }
}
