using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources: ")]
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceSFX;

    [Header("Volume Settings: ")]
    [SerializeField] Slider volumeSliderBGM;
    [SerializeField] Slider volumeSliderSFX;
    [SerializeField] Toggle musicMuteToggle;
    [SerializeField] Toggle sfxMuteToggle;
    public bool musicMute = false;
    public bool sfxMute = false;

    [SerializeField] AudioClip[] musicList;
    [SerializeField] AudioClip[] sfxList;



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

    public void PlaySoundEffect(int index)
    {
        audioSourceSFX.PlayOneShot(sfxList[index]);
    }

    public void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, sfxList.Length);
        audioSourceSFX.PlayOneShot(sfxList[randomIndex]);
    }

    public void PlaySoundMusic(int index)
    {
        audioSourceBGM.clip = musicList[index];
        audioSourceBGM.Play();
    }

    //IEnumerator PlayFootSteps()
    //{
        
    //}

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
        int currentSceneNumber = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneNumber < 0)
        {
            audioSourceBGM.clip = musicList[currentSceneNumber];
            audioSourceBGM.loop = true;
            audioSourceBGM.Play();
        }
    }

    private void SaveMusicPref()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSliderBGM.value);
        PlayerPrefs.SetFloat("sfxVolume", volumeSliderSFX.value);
    }

    private void LoadMusicPref()
    {
        if(audioSourceBGM.volume == 0) { musicMute = true;  }
        volumeSliderBGM.value = PlayerPrefs.GetFloat("musicVolume");
        volumeSliderSFX.value = PlayerPrefs.GetFloat("sfxVolume");
    }

    public void Mute(Toggle toggle)
    {

        if (toggle.name == "MusicMute")
        {
            if (musicMute == false) { musicMute = true; }
            else { musicMute = false; }
        }
        else if (toggle.name == "SFXMute")
        {
            if (sfxMute == false) { sfxMute = true; }
            else { sfxMute = false; }
        }
    }
}