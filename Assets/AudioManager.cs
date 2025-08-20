using UnityEngine;
using UnityEngine.Audio;
using System;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    private const string MasterVolumeKey = "MasterVolume";
    private const float DefaultVolume = 0.8f;

    public float CurrentVolume { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadVolume();
    }

    private void LoadVolume()
    {
        CurrentVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultVolume);
        SetMasterVolume(CurrentVolume);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        CurrentVolume = volume;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeFromSlider(float sliderValue)
    {
        SetMasterVolume(sliderValue);
    }
}