using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lake : MonoBehaviour
{
    [SerializeField] float transitionTime;

    [SerializeField] Color oilColor;
    [SerializeField] Color oilSpecularColor;

    [SerializeField] Color waterColor;
    [SerializeField] Color waterSpecularColor;


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

            rend.material.SetColor("_Tint", Color.Lerp(oilColor, waterColor, _percent));
            rend.material.SetColor("_SpecularColor", Color.Lerp(oilColor, waterColor, _percent));
        }
    }

    [ContextMenu("bruh")]
    public void Transition()
    {
        transition = true;

        timer = transitionTime;

        gameObject.layer = 4;
    }
}
