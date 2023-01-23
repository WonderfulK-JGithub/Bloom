using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullAnimEvent : MonoBehaviour
{
    [SerializeField] Seagull seagull;
    void StartFly()
    {
        seagull.StartFlying();
    }
}
