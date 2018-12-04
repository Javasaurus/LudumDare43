using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public bool clearPrefs;
    public static string MASTER_VOLUME = "Master";
    public static string AMBIENT_VOLUME = "Ambient";
    public static string SFX_VOLUME = "SFX";

    public float DefaultMasterVolume = 0f;
    public float DefaultAmbientVolume = 0f;
    public float DefaultSFXVolume = 0f;

    public float currentMasterVolume;
    public float currentambientVolume;
    public float currentSFXVolume;

    public Toggle pianoToggle;


    public Slider masterSlider;
    public Slider ambientSlider;
    public Slider sfxSlider;

    public static bool PIANO;

    public AudioMixer masterMixer;

    private void Start()
    {
        ResetConfig();
    }

    public void SaveConfig()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME, currentMasterVolume);
        PlayerPrefs.SetFloat(AMBIENT_VOLUME, currentambientVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, currentSFXVolume);
        PlayerPrefs.Save();
    }


    public void ResetConfig()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOLUME, DefaultMasterVolume));
        SetAmbientVolume(PlayerPrefs.GetFloat(AMBIENT_VOLUME, DefaultAmbientVolume));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOLUME, DefaultSFXVolume));
    }

    public void SetMasterVolume(float soundLevel)
    {
        if (masterSlider)
        {
            masterSlider.value = soundLevel;
        }
        currentMasterVolume = soundLevel;
        masterMixer.SetFloat(MASTER_VOLUME, soundLevel);
    }

    public void SetAmbientVolume(float soundLevel)
    {
        if (ambientSlider)
        {
            ambientSlider.value = soundLevel;
        }
        currentambientVolume = soundLevel;
        masterMixer.SetFloat(AMBIENT_VOLUME, soundLevel);
    }

    public void SetSFXVolume(float soundLevel)
    {
        if (sfxSlider)
        {
            sfxSlider.value = soundLevel;
        }
        currentSFXVolume = soundLevel;
        masterMixer.SetFloat(SFX_VOLUME, soundLevel);
    }

    public void SetChiptuneMusic()
    {
        PIANO = pianoToggle.isOn;
        GameObject.FindObjectOfType<ProceduralMusicChanger>().setPiano(PIANO);
    }


    public void Update()
    {
        if (clearPrefs)
        {
            PlayerPrefs.DeleteAll();
            clearPrefs = false;
        }
    }
}

