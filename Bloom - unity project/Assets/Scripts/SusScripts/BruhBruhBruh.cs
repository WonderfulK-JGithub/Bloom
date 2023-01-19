using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruhBruhBruh : MonoBehaviour
{
    void Complete()
    {
        FindObjectOfType<CollectibleCollectorScript>().RecycleComplete();
        print("adasd");
    }
}
