using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterIcon : MonoBehaviour
{
    Transform cameraTransform;
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        transform.LookAt(cameraTransform, Vector3.up);
    }
}
