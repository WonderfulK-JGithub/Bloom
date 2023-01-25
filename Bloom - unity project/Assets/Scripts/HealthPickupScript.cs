using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0,500 * Time.deltaTime, 0));
    }
}
