using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagg : MonoBehaviour
{
    public IgelkottMovement parent;
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation *= Quaternion.Euler(-90, 0, 0);
        GetComponent<Rigidbody>().isKinematic = false;

        Destroy(gameObject, parent.taggLifetime);

    }
    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.up * parent.taggSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.transform.gameObject.layer == 8)
        {
            parent.DamagePlayer(15);
        }
        else if (!other.CompareTag("tagg") && other.gameObject.layer != 9)
        {
            Destroy(gameObject);
        }

    }
}
