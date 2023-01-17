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
            transform.up = Vector3.Lerp(transform.up, hit.normal, 5 * Time.deltaTime);

            child.localRotation = Quaternion.Euler(0, parent.eulerAngles.y, 0);
        }

        //Debug.DrawRay(child.position + child.forward, -child.up + child.forward);
        parent.GetComponent<enemymovement>().brake = !GroundAhead();
    }

    bool GroundAhead()
    {
        RaycastHit hit;
        if (Physics.Raycast(child.position + (child.up * child.lossyScale.y / 2), -child.up + child.forward, out hit, 2))
        {
            if (hit.transform.gameObject.layer == 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
