using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlantCompletionHandler : MonoBehaviour
{
    public static PlantCompletionHandler current;

    [SerializeField] float plantReach;
    [SerializeField] float innerPlantReach;
    [SerializeField] float saturateSpeed;

    [Header("Completion")]
    [SerializeField] GameObject completion;
    [SerializeField] Slider completionBar;
    [SerializeField] float fillSpeed;
    [SerializeField] TextMeshProUGUI percentText;
    [SerializeField] float appearTime;
    [SerializeField] Gradient completionGradient;
    [SerializeField] Image barImage;

    ComputeBuffer plantDataBuffer;

    public List<Material> groundMaterials;

    Animator anim;

    Plant[] plants;

    Vector3[] plantCompletionGrid;

    int gridLength;
    int completeCount;

    List<int> plantsToSaturate = new List<int>();

    float appearTimer;
    void Awake()
    {
        anim = GetComponent<Animator>();

        current = this;

        plants = FindObjectsOfType<Plant>();

        gridLength = plants.Length;

        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].plantID = i;
        }


        plantCompletionGrid = new Vector3[gridLength];

        plantDataBuffer = new ComputeBuffer(plantCompletionGrid.Length, sizeof(float) * 3);
        plantDataBuffer.SetData(plantCompletionGrid);

        foreach (var groundMaterial in groundMaterials)
        {
            groundMaterial.SetFloat("_plantReach", plantReach);
            groundMaterial.SetFloat("_innerPlantReach", innerPlantReach);
            groundMaterial.SetInt("_bufferLength", gridLength);
            groundMaterial.SetBuffer("_plantBuffer", plantDataBuffer);
        }
    }

    private void Update()
    {
        bool _needUpdate = plantsToSaturate.Count > 0;

        for (int i = 0; i < plantsToSaturate.Count; i++)
        {
            int _index = plantsToSaturate[i];
            Vector3 _position = plants[_index].transform.position;

            float _value = plantCompletionGrid[_index].z;
            float _newValue = Mathf.MoveTowards(_value, 1f, saturateSpeed * Time.deltaTime);
            plantCompletionGrid[_index] = new Vector3(_position.x, _position.z, _newValue);
            if(_newValue == 1f)
            {
                plantsToSaturate.RemoveAt(i);
                i--;
            }
        }

        if (_needUpdate)
        {
            plantDataBuffer.SetData(plantCompletionGrid);
        }

        if (appearTimer >= 0f)
        {
            appearTimer -= Time.deltaTime;

            completion.SetActive(true);

            float _targetPercent = completeCount / (float)gridLength;

            completionBar.value = Mathf.MoveTowards(completionBar.value, _targetPercent, fillSpeed * Time.deltaTime);
            percentText.text = Mathf.Floor(_targetPercent * 100).ToString() + "%";

            barImage.color = completionGradient.Evaluate(completionBar.value);
        }
        else
        {
            completion.SetActive(false);

            if(completeCount == gridLength)
            {
                anim.Play("100");
                enabled = false;
                completion.SetActive(true);
            }
        }
    }

    public void SetGridBox(float _value, Vector3 _position, int _index)
    {
        plantCompletionGrid[_index] = new Vector3(_position.x, _position.z, 0f);
        plantDataBuffer.SetData(plantCompletionGrid);

        plantsToSaturate.Add(_index);

        completeCount++;

        appearTimer = appearTime;
    }

    private void OnDisable()
    {
        plantDataBuffer.Dispose();
    }

    void amorgos()
    {
        SceneManager.LoadScene(0);
    }
    
}
