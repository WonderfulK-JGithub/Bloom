using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlantCompletionHandler : MonoBehaviour
{
    public static PlantCompletionHandler current;

    [SerializeField] float plantReach;
    [SerializeField] float innerPlantReach;
    [SerializeField] float saturateSpeed;

    ComputeBuffer plantDataBuffer;

    public Material groundMaterial;

    Plant[] plants;

    Vector3[] plantCompletionGrid;

    int gridLength;

    List<int> plantsToSaturate = new List<int>();

    void Awake()
    {
        
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

        groundMaterial.SetFloat("_plantReach", plantReach);
        groundMaterial.SetFloat("_innerPlantReach", innerPlantReach);
        groundMaterial.SetInt("_bufferLength", gridLength);
        groundMaterial.SetBuffer("_plantBuffer", plantDataBuffer);
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
    }

    public void SetGridBox(float _value, Vector3 _position, int _index)
    {
        plantCompletionGrid[_index] = new Vector3(_position.x, _position.z, 0f);
        plantDataBuffer.SetData(plantCompletionGrid);

        plantsToSaturate.Add(_index);
    }

    private void OnDisable()
    {
        plantDataBuffer.Dispose();
    }
}
