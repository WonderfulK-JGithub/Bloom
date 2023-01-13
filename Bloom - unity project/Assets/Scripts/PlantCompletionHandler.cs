using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlantCompletionHandler : MonoBehaviour
{
    public static PlantCompletionHandler current;

    [SerializeField] int gridSize;
    [SerializeField] float unitsPerGrid;
    public RenderTexture completionTexture;
    [SerializeField] ComputeShader completionCompute;


    public List<Material> grounds;
    

    float[] plantCompletionGrid;
    void Awake()
    {
        plantCompletionGrid = new float[gridSize * gridSize];
        current = this;
        //completionTexture.enableRandomWrite = true;

        completionTexture = new RenderTexture(gridSize, gridSize, 24);
        completionTexture.enableRandomWrite = true;
        completionTexture.filterMode = FilterMode.Point;
        completionTexture.Create();
        

        completionCompute.SetInt("gridSize", gridSize);

        foreach (var item in grounds)
        {
            item.SetTexture("_CompletionTexture", completionTexture);
            item.SetFloat("_UnitsPerPixel", unitsPerGrid);
            item.SetVector("_CompletionTexel", completionTexture.texelSize);
            item.SetFloat("_GridSize", gridSize);
            
        }
    }

    
    public void SetGridBox(float _value, Vector3 _position)
    {
        Vector2Int _gridPosition = new Vector2Int(Mathf.RoundToInt(_position.x / unitsPerGrid) + gridSize / 2, Mathf.RoundToInt(_position.z / unitsPerGrid) + gridSize / 2);

        plantCompletionGrid[_gridPosition.y * gridSize + _gridPosition.x] = 1f;

        ComputeBuffer _infoBuffer = new ComputeBuffer(plantCompletionGrid.Length, sizeof(float));
        _infoBuffer.SetData(plantCompletionGrid);

        completionCompute.SetBuffer(0, "Grid", _infoBuffer);
        completionCompute.SetTexture(0, "Result", completionTexture);

        completionCompute.Dispatch(0, Mathf.CeilToInt(gridSize / 8), Mathf.CeilToInt(gridSize / 8), 1);

        _infoBuffer.Dispose();

    }
}
