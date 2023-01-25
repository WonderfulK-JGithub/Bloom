using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour,IWaterable
{
    [SerializeField] float meshHeight;
    [SerializeField] float meshStartY;
    //[SerializeField] Material mat;
    [SerializeField] float fillPerShoot;
    [SerializeField] float fillSpeed;

    [SerializeField] List<Lake> lakes;
    [SerializeField] GameObject waterIcon;

    [SerializeField] Renderer rend;

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

    
    float shaderValue;

    private void Awake()
    {
        //rend = GetComponent<Renderer>();

        foreach (var _material in rend.materials)
        {
            _material.SetFloat("_Height", meshHeight);
            _material.SetFloat("_StartY", meshStartY + transform.position.y);
        }

        //rend.material.SetFloat("_Height", meshHeight);
        //rend.material.SetFloat("_StartY", meshStartY + transform.position.y);
        shaderValue = 0f;
    }

    private void Update()
    {
        //shaderValue = Mathf.MoveTowards(shaderValue, WaterValue * suspect * 2f - suspect,fillSpeed * Time.deltaTime);
        shaderValue = Mathf.Lerp(shaderValue, WaterValue,  fillSpeed * Time.deltaTime * 60f);
        //rend.material.SetFloat("_Value", shaderValue);

        foreach (var _material in rend.materials)
        {
            _material.SetFloat("_Value", shaderValue);
        }
    }

    public void Water()
    {
        if (WaterValue == 1f) return;

        WaterValue += fillPerShoot;

        if (WaterValue == 1f)
        {
            AudioManager.current.PlaySound(AudioManager.AudioNames.FlowerGreen, transform.position);
            PlantCompletionHandler.current.SetGridBox(WaterValue, transform.position, plantID);
            waterIcon.SetActive(false);
            foreach (var lake in lakes)
            {
                lake.Transition();
            }

            //För saturation - Max
            FindObjectOfType<PlayerCameraScript>().wateredPlants.Add(this);
        }
    }

    
}
