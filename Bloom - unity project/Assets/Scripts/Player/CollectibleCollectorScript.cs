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
    int onLevel;
    [SerializeField] int neededForUpgrades;
    [SerializeField] GameObject biggestBird;
    [SerializeField] Slider upgradeSlider;

    [Header("Lathet")]
    [SerializeField] GameObject[] checkmarks;


    int collectibles = 0;
    int recycled = 0;
    int totalRecycled = 0;

    TextMeshProUGUI collectibleText;

    public override void Awake()
    {
        base.Awake();

        collectibleText = GameObject.Find("CollectibleText").GetComponent<TextMeshProUGUI>();

        onLevel = FindObjectsOfType<CollectibleScript>().Length;
    }

    private void Start()
    {
        totalRecycled = PlayerPrefs.GetInt("TotalRecycled");
        CheckUpgrades();
        h.Damage(-100);
        shooting.FillAmmo();

        Debug.Log(totalRecycled);

        if (totalRecycled == 0)
            upgradeSlider.transform.parent.gameObject.SetActive(false);
        else
            Invoke(nameof(TurnOffSlider), 3);
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
                totalRecycled += collectibles;
                PlayerPrefs.SetInt("TotalRecycled", totalRecycled);
                upgradeSlider.transform.parent.gameObject.SetActive(true);

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
        CheckUpgrades();

        if (recycled < onLevel)
        {
            xOfxCansText.text = recycled.ToString() + "/" + onLevel.ToString();
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

    void CheckUpgrades()
    {
        float percent = (float)totalRecycled / (float)neededForUpgrades;
        Debug.Log(percent);
        upgradeSlider.value = percent;
        if (percent >= 0.25f) { h.SetMaxHealth(150); checkmarks[0].SetActive(true); }
        if (percent >= 0.5f) { shooting.SetMaxAmmo(150); checkmarks[1].SetActive(true); }
        if (percent >= 0.75f) { h.SetMaxHealth(200); checkmarks[2].SetActive(true); }
        if (percent >= 1f) { shooting.SetMaxAmmo(200); checkmarks[3].SetActive(true); }

        Invoke(nameof(TurnOffSlider), 5);
    }

    void TurnOffSlider()
    {
        upgradeSlider.transform.parent.gameObject.SetActive(false);
    }
}
