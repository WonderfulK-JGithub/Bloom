using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearAfterTime : MonoBehaviour
{
    [SerializeField] float time;

    float timer;
    protected virtual void Awake()
    {
        timer = time;
    }

    private void Update()
    {
        if (PauseMenu.paused) return;

        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
