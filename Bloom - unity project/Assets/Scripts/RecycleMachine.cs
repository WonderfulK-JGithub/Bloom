using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleMachine : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void Recycle()
    {
        anim.Play("Recycling");
        AudioManager.current.PlaySound(AudioManager.AudioNames.TrashCollect);
    }   
}
