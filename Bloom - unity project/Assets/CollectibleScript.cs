using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    [SerializeField] List<GameObject> trash = new List<GameObject>();

    private void Start()
    {
        GameObject go = Instantiate(trash[Random.Range(0, trash.Count)], transform);
        go.transform.localScale = Vector3.one * 0.3f;
    }
}
