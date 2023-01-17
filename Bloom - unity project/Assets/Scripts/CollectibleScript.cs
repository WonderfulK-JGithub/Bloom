using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    [SerializeField] List<GameObject> trash = new List<GameObject>();
    [SerializeField] float rotationSpeed;

    float susDomesticus;

    private void Start()
    {
        GameObject go = Instantiate(trash[Random.Range(0, trash.Count)], transform);
        go.transform.localScale = Vector3.one * 0.3f;
    }

    private void Update()
    {
        susDomesticus += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Euler( new Vector3(0f, susDomesticus, -45f));
    }

    public void Collect()
    {
        Destroy(this.gameObject);
    }
}
