using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleCollectorScript : MonoBehaviour
{
    [SerializeField] float recycleRange;
    [SerializeField] LayerMask recycleLayer;
    [SerializeField] Transform cameraTransform;

    
    int collectibles = 0;

    TextMeshProUGUI collectibleText;

    private void Awake()
    {
        collectibleText = GameObject.Find("CollectibleText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward,out RaycastHit _hit, recycleRange, recycleLayer))
        {
            if (Input.GetButtonDown("Interact"))
            {
                RecycleMachine _recycle = _hit.collider.GetComponent<RecycleMachine>();

                _recycle.Recycle();
            }
        }
    }

    public void Collect(CollectibleScript collectible)
    {
        collectibles++;
        collectible.Collect();

        if(collectibleText != null)
            collectibleText.text = collectibles.ToString();

        AudioManager.current.PlaySound(AudioManager.AudioNames.TrashCollect);
    }
}
