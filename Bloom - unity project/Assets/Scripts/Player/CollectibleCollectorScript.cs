using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleCollectorScript : PlayerBaseScript
{
    [SerializeField] float recycleRange;
    [SerializeField] LayerMask recycleLayer;
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject recycleText;
    [SerializeField] TextMeshProUGUI xOfxCansText;
    [SerializeField] GameObject upgradeText;
    [SerializeField] int neededForUpgrade;
    [SerializeField] GameObject biggestBird;


    int collectibles = 0;
    int recycled;

    TextMeshProUGUI collectibleText;

    public override void Awake()
    {
        collectibleText = GameObject.Find("CollectibleText").GetComponent<TextMeshProUGUI>();

        neededForUpgrade = FindObjectsOfType<CollectibleScript>().Length;
    }

    private void Update()
    {
        if(collectibles > 0 && Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit _hit, recycleRange, recycleLayer))
        {
            if (Input.GetButtonDown("Interact"))
            {
                RecycleMachine _recycle = _hit.collider.GetComponent<RecycleMachine>();

                _recycle.Recycle();

                recycled += collectibles;

                collectibles = 0;
                collectibleText.text = collectibles.ToString();
            }
            recycleText.SetActive(true);
        }
        else
        {
            recycleText.SetActive(false);
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

    public void RecycleComplete()
    {
        if(recycled < neededForUpgrade)
        {
            xOfxCansText.text = recycled.ToString() + "/" + neededForUpgrade.ToString();
            xOfxCansText.gameObject.SetActive(true);
            Invoke(nameof(Mogus), 2f);
        }
        else
        {
            upgradeText.SetActive(true);
            Invoke(nameof(Mogus), 2f);
            biggestBird.SetActive(true);
        }
    }

    void Mogus()
    {
        xOfxCansText.gameObject.SetActive(false);
        upgradeText.SetActive(false);
    }
}
