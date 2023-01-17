using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleCollectorScript : MonoBehaviour
{
    int collectibles = 0;

    TextMeshProUGUI collectibleText;

    private void Awake()
    {
        collectibleText = GameObject.Find("CollectibleText").GetComponent<TextMeshProUGUI>();
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
