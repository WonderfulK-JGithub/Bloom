using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffect : DissapearAfterTime
{
    [SerializeField] ParticleSystem[] ps;

    protected override void Awake()
    {
        base.Awake();

        PauseMenu.OnPause += Paused;
    }

    void Paused(bool _pause)
    {
        if (_pause)
        {
            foreach (var _ps in ps)
            {
                _ps.Pause();
            }
        }
        else
        {
            foreach (var _ps in ps)
            {
                _ps.Play();
            }
        }
    }

    private void OnDestroy()
    {
        PauseMenu.OnPause -= Paused;
    }
}
