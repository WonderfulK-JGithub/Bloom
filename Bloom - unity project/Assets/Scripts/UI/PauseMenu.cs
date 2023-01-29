using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public static Action<bool> OnPause;
    public static bool paused;

    [SerializeField] GameObject overlay;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] TextMeshProUGUI sfxPercent;
    [SerializeField] TextMeshProUGUI musicPercent;
    [SerializeField] TextMeshProUGUI sensPercent;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sensSlider;

    [SerializeField] float maxSensitivity;
    [SerializeField] float minSensitivity;
    [SerializeField] float sensPower;

    [SerializeField] AudioMixer mixer;

    PlayerCameraScript cameraScript;

    

    private void Awake()
    {
        cameraScript = FindObjectOfType<PlayerCameraScript>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        paused = !paused;

        OnPause?.Invoke(paused);

        if (paused)
        {
            pausePanel.SetActive(true);
            overlay.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else
        {
            pausePanel.SetActive(false);
            overlay.SetActive(false);
            settingsPanel.SetActive(false);

            SaveSettings();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
    }

    public void Pause(bool _pause)
    {
        paused = _pause;

        OnPause?.Invoke(paused);
    }

    public void Quit()
    {
        SceneTransition.current.EnterScene(0);

        SaveSettings();

        enabled = false;
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);

        SettingsData _data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(Application.persistentDataPath + SettingsData.saveName));

        sfxSlider.value = _data.sfxVolume;
        sfxPercent.text = Mathf.Round(sfxSlider.value * 100).ToString() + "%";

        musicSlider.value = _data.musicVolume;
        musicPercent.text = Mathf.Round(musicSlider.value * 100).ToString() + "%";

        sensSlider.value = _data.mouseSensitivity;
        sensPercent.text = Mathf.Round(sensSlider.value * 100).ToString() + "%";
    }

    void SaveSettings()
    {
        SettingsData _data = new SettingsData()
        {
            sfxVolume = sfxSlider.value,
            musicVolume = musicSlider.value,
            mouseSensitivity = sensSlider.value,
        };

        File.WriteAllText(Application.persistentDataPath + SettingsData.saveName, JsonUtility.ToJson(_data));
    }

    public void ChangeMusicVolume()
    {
        float _volume = musicSlider.value;

        mixer.SetFloat("musicvol", Mathf.Log10(_volume) * 40);
        musicPercent.text = Mathf.Round(_volume * 100).ToString() + "%";
    }

    public void ChangeSFXVolume()
    {
        float _volume = sfxSlider.value;

        mixer.SetFloat("sfxvol", Mathf.Log10(_volume) * 40);
        sfxPercent.text = Mathf.Round(_volume * 100).ToString() + "%";
    }

    public void ChangeMouseSense()
    {
        float _sensPower = sensSlider.value;

        cameraScript.sensitivity = Remap(0f,1f,minSensitivity,maxSensitivity,_sensPower);
        sensPercent.text = Mathf.Round(_sensPower * 100).ToString() + "%";
    }

    float Remap(float _origFrom, float _origTo, float _targetFrom, float _targetTo, float _value)
    {
        float _percent = Mathf.InverseLerp(_origFrom, _origTo, _value);
        return Mathf.Lerp(_targetFrom, _targetTo, _percent);
    }

    private void OnDestroy()
    {
        paused = false;
    }
}

public class SettingsData
{
    public static string saveName = "/settings.sav"; //använd såklart Application.persistentDataPath

    public float sfxVolume;
    public float musicVolume;
    public float mouseSensitivity;

    public SettingsData()
    {
        sfxVolume = 0.5f;
        musicVolume = 0.5f;
        mouseSensitivity = 0.5f;
    }
}
