using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public enum SoundType
{ 
    BGM,
    MeleeAttack,
    RangedAttack,
    FootSteps,
    Jump,
    OpenDoor,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] Slider volumeSliderBGM;
    [SerializeField] Slider volumeSliderSFX;

    [SerializeField] SoundList[] soundList;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceSFX;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        else
        {
            LoadMusicPref();
        }

        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
        }
        else
        {
            LoadMusicPref();
        }
    }

    void Update()
    {
        OnSceneLoad();
    }

    public void PlaySound(SoundType sound, float volume = 1)
    {
        //audioSourceSFX.PlayOneShot(soundList[(int)sound], volume);
    }

    public void ChangeBGMVolume()
    {
        audioSourceBGM.volume = volumeSliderBGM.value;
        SaveMusicPref();
    }

    public void ChangeSFXVolume()
    {
        audioSourceSFX.volume = volumeSliderSFX.value;
        SaveMusicPref();
    }

    public void OnSceneLoad()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(audioSourceBGM.clip == null)
            {
                //audioSourceBGM.clip = instance.soundList[0];
                audioSourceBGM.loop = true;
                audioSourceBGM.Play();
            }
        }
    }

    private void SaveMusicPref()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSliderBGM.value);
        PlayerPrefs.SetFloat("sfxVolume", volumeSliderSFX.value);
    }

    private void LoadMusicPref()
    {
        volumeSliderBGM.value = PlayerPrefs.GetFloat("musicVolume");
        volumeSliderSFX.value = PlayerPrefs.GetFloat("sfxVolume");
    }

}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] AudioClip[] sounds;
}

