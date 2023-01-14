using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlantCompletionHandler : MonoBehaviour
{
    public static PlantCompletionHandler current;

    [SerializeField] float plantReach;

    ComputeBuffer plantDataBuffer;

    public Material groundMaterial;

    Plant[] plants;

    Vector3[] plantCompletionGrid;

    int gridLength;

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
        groundMaterial.SetInt("_bufferLength", gridLength);
        groundMaterial.SetBuffer("_plantBuffer", plantDataBuffer);
    }

    
    public void SetGridBox(float _value, Vector3 _position, int _index)
    {
        plantCompletionGrid[_index] = new Vector3(_position.x, _position.z, _value);
        plantDataBuffer.SetData(plantCompletionGrid);
    }

    private void OnDisable()
    {
        plantDataBuffer.Dispose();
    }
}
