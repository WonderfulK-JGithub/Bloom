using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioNames { WaterSpray, WaterFill, TrashCollect, Footstep1, Footstep2, Footstep3, WaterSplash, HeartSound, PlayerDamage,OilSplash,FlowerGreen,Tagg,KaninHopp,Seagull,Seagull2,JumpInWater,JumpInOil };

    public List<AudioClip> audioClips = new List<AudioClip>();

    public static AudioManager current;

    [SerializeField] AudioSource sfx2D;

    [SerializeField] AudioSource footstepSFX;

    [SerializeField] GameObject sourcePrefab;

    private void Awake()
    {
        //sfx2D = GetComponent<AudioSource>();

        current = this;
    }

    public void PlaySound(AudioNames audioName)
    {
        sfx2D.PlayOneShot(audioClips[(int)audioName]);
    }

    public void PlaySound(AudioNames audioName, Vector3 position, float lifetime = 1f)
    {
        //GameObject go = new GameObject("SFX", typeof(AudioSource));
        //go.transform.position = position;

        GameObject go = Instantiate(sourcePrefab, position, Quaternion.identity);

        AudioSource src = go.GetComponent<AudioSource>();
        src.spatialBlend = 1;
        src.PlayOneShot(audioClips[(int)audioName]);
        
        Destroy(go, lifetime);
    }

    public void PlayFootStep()
    {
        footstepSFX.Stop();
        footstepSFX.PlayOneShot(audioClips[Random.Range(3, 6)]);
    }
}
