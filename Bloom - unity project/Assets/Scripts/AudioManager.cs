using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioNames { WaterSpray, WaterFill, TrashCollect };

    public List<AudioClip> audioClips = new List<AudioClip>();

    public static AudioManager current;

    AudioSource sfx2D;
    private void Awake()
    {
        sfx2D = GetComponent<AudioSource>();

        current = this;
    }

    public void PlaySound(AudioNames audioName)
    {
        sfx2D.PlayOneShot(audioClips[(int)audioName]);
    }

    public void PlaySound(AudioNames audioName, Vector3 position)
    {
        GameObject go = new GameObject("SFX", typeof(AudioSource));
        go.transform.position = position;

        AudioSource src = go.GetComponent<AudioSource>();
        src.spatialBlend = 1;
        src.PlayOneShot(audioClips[(int)audioName]);

        Destroy(go, 2);
    }
}
