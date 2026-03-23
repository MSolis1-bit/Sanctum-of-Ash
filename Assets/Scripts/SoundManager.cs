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

    [SerializeField] AudioClip[] musicList;
    [SerializeField] AudioClip[] sfxList;

    private float musicBeforeMute = 1;
    private float sfxBeforeMute = 1;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one SoundManager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
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
        volumeSliderBGM.value = PlayerPrefs.GetFloat("musicVolume");
        volumeSliderSFX.value = PlayerPrefs.GetFloat("sfxVolume");
    }

    public void Mute(Toggle toggle)
    {

        if (toggle.name == "MusicMute")
        {
            if (toggle.isOn == true) { musicBeforeMute = audioSourceBGM.volume; audioSourceBGM.volume = 0; SaveMusicPref(); }
            else { audioSourceBGM.volume = musicBeforeMute; SaveMusicPref(); }
        }
        else if (toggle.name == "SFXMute")
        {
            if (toggle.isOn == true) { sfxBeforeMute = audioSourceSFX.volume; audioSourceSFX.volume = 0; SaveMusicPref(); }
            else { audioSourceSFX.volume = sfxBeforeMute; SaveMusicPref(); }
        }
    }
}