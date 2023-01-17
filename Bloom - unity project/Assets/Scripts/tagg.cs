using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagg : MonoBehaviour
{
    [HideInInspector] public IgelkottMovement parent;
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation *= Quaternion.Euler(-90, 0, 0);
        GetComponent<Rigidbody>().isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.up * parent.taggSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
