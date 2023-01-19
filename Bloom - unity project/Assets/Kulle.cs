using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kulle : MonoBehaviour
{
    public float minimumheight = -5;
    public float maxmumheight = 10;

    private void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            transform.position += new Vector3(0, Random.Range(minimumheight, 0));
        }
        else
        {
            transform.position += new Vector3(0, Random.Range(0, maxmumheight), 0);
        }
    }
}
