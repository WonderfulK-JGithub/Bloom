using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.IO;

public class MenuAnimations : MonoBehaviour
{
    public float waitBeforeIntro = 1;
    public float logoFadeSpeed = 1;
    public float waitBeforeInput = 1;
    public float inputTextFadeInSpeed = 2;
    public float inputTextMoveDownSpeed = 1;
    public float inputTextFadeOutSpeed = 2;
    public float logoMoveSpeed = 1;
    public float buttonFadeSpeed = 2;
    public float buttonFadeDelay = 0.1f;
    public float sceneLoadTime = 1;
    bool awaitInput = false;
    public float screenAnimationSpeed = 1;
    [Space]
    public RectTransform[] windows;
    public Button[] animationButtons;
    [Space]
    public Image logo;
    public RectTransform logoTargetPos;
    public Image bg;
    public TextMeshProUGUI anyKeyText;
    public GameObject buttonParent;
    private Image[] buttons;
    private Vector3 centerPos;
    public Image black;

    public AudioMixer mixer;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
    [SerializeField] TextMeshProUGUI sensText;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider sensSlider;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        logoTargetPos.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        centerPos = windows[0].localPosition;
        black.gameObject.SetActive(false);

        buttons = buttonParent.GetComponentsInChildren<Image>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].color -= new Color(0, 0, 0, 1);
            buttons[i].GetComponent<Button>().enabled = false;
        }

        bg.color = new Color(0, 0, 0, 1);
        anyKeyText.color = new Color(anyKeyText.color.r, anyKeyText.color.b, anyKeyText.color.g, 0);
        logo.color = new Color(1, 1, 1, 0);

        StartCoroutine(FadeLogo());

        GetData();
    }

    void GetData()
    {
        SettingsData _data;
        if (File.Exists(Application.persistentDataPath + SettingsData.saveName))
        {
            _data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(Application.persistentDataPath + SettingsData.saveName));
        }
        else
        {
            _data = new SettingsData();
        }

        sfxSlider.value = _data.sfxVolume;
        sfxText.text = Mathf.Round(sfxSlider.value * 100).ToString() + "%";

        musicSlider.value = _data.musicVolume;
        musicText.text = Mathf.Round(musicSlider.value * 100).ToString() + "%";

        sensSlider.value = _data.mouseSensitivity;
        sensText.text = Mathf.Round(sensSlider.value * 100).ToString() + "%";

        mixer.SetFloat("musicvol", Mathf.Log10(_data.musicVolume) * 40);
        mixer.SetFloat("sfxvol", Mathf.Log10(_data.sfxVolume) * 40);
    }

    void SaveData()
    {
        SettingsData _data = new SettingsData()
        {
            sfxVolume = sfxSlider.value,
            musicVolume = musicSlider.value,
            mouseSensitivity = sensSlider.value,
        };

        File.WriteAllText(Application.persistentDataPath + SettingsData.saveName, JsonUtility.ToJson(_data));
    }

    IEnumerator FadeLogo()
    {
        yield return new WaitForSeconds(waitBeforeIntro);
        while (logo.color.a < 1)
        {
            bg.color += new Color(Time.deltaTime * logoFadeSpeed, Time.deltaTime * logoFadeSpeed, Time.deltaTime * logoFadeSpeed, 1);
            logo.color += new Color(0, 0, 0, Time.deltaTime * logoFadeSpeed);
            yield return 0;
        }
        yield return new WaitForSeconds(waitBeforeInput);
        while (anyKeyText.color.a < 1)
        {
            anyKeyText.color += new Color(anyKeyText.color.r, anyKeyText.color.b, anyKeyText.color.g, Time.deltaTime * inputTextFadeInSpeed);
            yield return 0;
        }
        awaitInput = true;
    }

    private void Update()
    {
        if (awaitInput)
        {
            if (Input.anyKeyDown)
            {
                awaitInput = false;
                StartCoroutine(LoadMenu());
            }
        }
    }

    IEnumerator LoadMenu()
    {
        while (anyKeyText.color.a > 0)
        {
            anyKeyText.rectTransform.localPosition -= new Vector3(0, Time.deltaTime * inputTextMoveDownSpeed * 100, 0);
            anyKeyText.color -= new Color(0, 0, 0, Time.deltaTime * inputTextFadeOutSpeed);
            yield return 0;
        }

        float t = 0;
        Vector3 a = logo.rectTransform.localPosition;
        while (t < 1)
        {
            logo.rectTransform.localPosition = Vector3.Lerp(a, logoTargetPos.localPosition, t);
            t += Time.deltaTime * logoMoveSpeed;
            yield return 0;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            yield return new WaitForSeconds(buttonFadeDelay);
            StartCoroutine(FadeButton(buttons[i]));
        }
    }

    IEnumerator FadeButton(Image button)
    {
        Vector3 targetPos = button.rectTransform.localPosition;
        button.rectTransform.localPosition -= new Vector3(0, 500, 0);

        float t = 0;

        while (t < 1)
        {
            button.color = new Color(1, 1, 1, t);

            button.rectTransform.localPosition = Vector3.Lerp(button.rectTransform.localPosition, targetPos, t);
            t += Time.deltaTime * buttonFadeSpeed;
            yield return 0;
        }

        button.GetComponent<Button>().enabled = true;
    }
    IEnumerator LoadSceneAnim(int scene)
    {
        float t = 0;
        black.gameObject.SetActive(true);

        while (t < 1)
        {
            black.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), t);
            t += Time.deltaTime * sceneLoadTime;
            yield return 0;
        }
        SceneManager.LoadScene(scene);
    }

    IEnumerator CenterCameraToScreen(int screen)
    {
        Vector3 lastPos = windows[screen].localPosition;
        Vector3 firstPos = windows[screen].localPosition;
        float t = 0;

        foreach (var b in animationButtons)
        {
            b.interactable = false;
        }

        while (t <= 1)
        {
            t += Time.deltaTime * screenAnimationSpeed;

            windows[screen].localPosition = Vector3.Lerp(firstPos, centerPos, t);

            for (int i = 0; i < windows.Length; i++)
            {
                if (i != screen)
                {
                    windows[i].localPosition += windows[screen].localPosition - lastPos;
                }
            }

            lastPos = windows[screen].localPosition;
            yield return 0;
        }


        foreach (var b in animationButtons)
        {
            b.interactable = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenScreen(int screen)
    {
        StartCoroutine(CenterCameraToScreen(screen));

        if(screen == 0)
        {
            SaveData();
        }
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAnim(index));
    }

    public void ChangeMusicVolume(float volume)
    {
        mixer.SetFloat("musicvol", Mathf.Log10(volume) * 40);
        musicText.text = Mathf.Round(volume * 100).ToString() + "%";
    }

    public void ChangeSFXVolume(float volume)
    {
        mixer.SetFloat("sfxvol", Mathf.Log10(volume) * 40);
        sfxText.text = Mathf.Round(volume * 100).ToString() + "%";
    }

    public void ChangeSensitivity()
    {
        sfxText.text = Mathf.Round(sfxSlider.value * 100).ToString() + "%";
    }

    public void ToggleCS(bool toggle)
    {
        PlayerPrefs.SetInt("CameraShakeBool", System.Convert.ToInt32(toggle));
    }
}
