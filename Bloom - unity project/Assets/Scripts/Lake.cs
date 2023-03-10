using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lake : MonoBehaviour
{
    [SerializeField] float transitionTime;

    [SerializeField] Color oilColor;
    [SerializeField] Color oilSpecularColor;
    [SerializeField] float oilEmissionPower;
    [SerializeField] float oilSeaThroughPower;

    [SerializeField] Color waterColor;
    [SerializeField] Color waterSpecularColor;
    [SerializeField] float waterEmissionPower;
    [SerializeField] float waterSeaThroughPower;

    [SerializeField] GameObject splashParticleOil;
    [SerializeField] GameObject splashParticleWater;

    float timer;
    bool transition;

    Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (transition)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                timer = 0f;
                transition = false;
            }

            float _percent = 1f - timer / transitionTime;

            //rend.material.SetColor("_Tint", Color.Lerp(oilColor, waterColor, _percent));
            //rend.material.SetColor("_SpecularColor", Color.Lerp(oilColor, waterColor, _percent));
            //rend.material.SetFloat("_EmissionPower", Mathf.Lerp(oilEmissionPower, waterEmissionPower, _percent));
            rend.material.SetFloat("_SeaThroughPower", Mathf.Lerp(oilSeaThroughPower, waterSeaThroughPower, _percent));
        }
    }

    [ContextMenu("bruh")]
    public void Transition()
    {
        gameObject.tag = "Untagged";

        transition = true;

        timer = transitionTime;

        gameObject.layer = 4;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject _newPS;
        if (gameObject.CompareTag("Untagged"))
        {
            _newPS = Instantiate(splashParticleWater, other.transform.position, Quaternion.identity);
            AudioManager.current.PlaySound(AudioManager.AudioNames.JumpInWater, other.transform.position);
        }
        else
        {
            _newPS = Instantiate(splashParticleOil, other.transform.position, Quaternion.identity);
            AudioManager.current.PlaySound(AudioManager.AudioNames.JumpInOil, other.transform.position);
        }

        Destroy(_newPS, 0.7f);

        
    }
}
