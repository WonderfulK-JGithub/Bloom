using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour,IWaterable
{
    [SerializeField] float meshHeight;
    //[SerializeField] Material mat;
    [SerializeField] float fillPerShoot;
    [SerializeField] float fillSpeed;

    Renderer rend;

    public int plantID;

    float waterValue;
    public float WaterValue
    {
        get
        {
            return waterValue;
        }
        set
        {
            waterValue = Mathf.Clamp(value, 0f, 1f);
        }
    }

    float suspect = 0.7f;
    float shaderValue;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material.SetFloat("_Height", meshHeight);
        shaderValue = -suspect;
    }

    private void Update()
    {
        //shaderValue = Mathf.MoveTowards(shaderValue, WaterValue * suspect * 2f - suspect,fillSpeed * Time.deltaTime);
        shaderValue = Mathf.Lerp(shaderValue, WaterValue * suspect * 2f - suspect,  fillSpeed * Time.deltaTime * 60f);
        rend.material.SetFloat("_Value", shaderValue);
    }

    public void Water()
    {
        WaterValue += fillPerShoot;

        PlantCompletionHandler.current.SetGridBox(WaterValue, transform.position,plantID);
    }

    
}
