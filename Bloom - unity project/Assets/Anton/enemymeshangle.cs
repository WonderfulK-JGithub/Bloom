using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymeshangle : MonoBehaviour
{
    public Transform parent;
    public Transform child;
    void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10, LayerMask.GetMask("Ground")))
        {
            transform.up = Vector3.Lerp(transform.up, hit.normal, 0.2f);

            child.rotation = Quaternion.Euler(parent.position.x, Vector3.Lerp(transform.up, hit.normal, 0.2f).y, parent.position.z);
        }
        
    }
}
