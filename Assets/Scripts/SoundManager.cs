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

    [SerializeField] AudioClip[] musicList;
    [SerializeField] AudioClip[] sfxList;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceSFX;

    public bool musicMute = false;
    public bool sfxMute = false;


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

        OnSceneLoad();
    }

    void Update()
    {

    }

    public void PlaySound(SoundType sound, float volume = 1)
    {
        audioSourceSFX.PlayOneShot(sfxList[(int)sound], volume);
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
                audioSourceBGM.clip = instance.musicList[0];
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

    public void Mute(Toggle toggle)
    {
        if(toggle.name == "MusicMute")
        {
            musicMute = true;
            audioSourceBGM.volume = 0;
            volumeSliderBGM.value = 0;
        } 
        else if(toggle.name == "SFXMute")
        {
            sfxMute = true;
            audioSourceSFX.volume = 0;
            volumeSliderSFX.value = 0;
        }
    }
}