using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenu : MonoBehaviour
{
    public static Action<bool> OnPause;
    bool paused;

    [SerializeField] GameObject PausePanel;

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        paused = !paused;

        OnPause?.Invoke(paused);
    }
}
