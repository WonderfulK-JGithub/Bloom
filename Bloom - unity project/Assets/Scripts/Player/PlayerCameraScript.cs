using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCameraScript : MonoBehaviour
{
    [Header("Parameters")]
    public float sensitivity = 100f;

    [Header("Headbobbing")]
    [SerializeField] bool headBob = true;
    [SerializeField] Vector2 headBobScale;
    [SerializeField] Vector2 headBobSpeed;
    [SerializeField] float headBobReturnSpeed;
    Vector3 headBobOffset = Vector3.zero;

    [Header("Screenshake")]
    [SerializeField] float screenShakeTime;
    [SerializeField] float screenShakeMagnitude;

    [Header("Saturation")]
    [SerializeField] float saturationSpeed;
    [SerializeField] float greenSaturation;
    [SerializeField] float notGreenSaturation;

    [Header("References")]
    [SerializeField] Camera cam;
    Rigidbody rb;
    public Transform deathObj;

    //General
    public static bool canLook = true;
    float xRot, yRot;

    //Camera shake
    float shakeTimer;
    float shakePower;
    float powerReduction;
    public static bool cameraShake = true;

    //Saturation
    ColorAdjustments colAd;
    float targetSaturation = 0;

    [HideInInspector] public List<Plant> wateredPlants = new List<Plant>();
    float plantReach = 15; //Man skulle kunna göra att den här kollar efter plantcompletionhandlerns range

    private void Awake()
    {
        //wateredPlants.AddRange(FindObjectsOfType<Plant>());

        cameraShake = System.Convert.ToBoolean(PlayerPrefs.GetInt("CameraShakeBool", 1));

        //Saturation
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
        //Saturation
        HandleSaturation();

        //Look
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

    void HandleSaturation()
    {
        //Saturation
        bool b = false;
        foreach (Plant plant in wateredPlants)
        {
            if (Vector3.Distance(transform.position, plant.transform.position) < plantReach)
            {
                b = true;
                break;
            }
        }
        targetSaturation = b ? greenSaturation : notGreenSaturation;

        if (colAd != null)
        {
            colAd.saturation.value = Mathf.Lerp(colAd.saturation.value, ((targetSaturation + 100f) * PlayerHealthScript.saturationMultiplier) - 100f, saturationSpeed * Time.deltaTime);
        }
    }
}
