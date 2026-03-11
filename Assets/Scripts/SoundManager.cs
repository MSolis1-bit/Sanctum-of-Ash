using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] Slider volumeSlider;

    [SerializeField] AudioClip[] soundList;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceSFX;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        OnSceneLoad();
    }

    public void PlaySound(SoundType sound, float volume = 1)
    {
        audioSourceSFX.PlayOneShot(soundList[(int)sound], volume);
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void OnSceneLoad()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(audioSourceBGM.clip == null)
            {
                audioSourceBGM.clip = instance.soundList[0];
                audioSourceBGM.loop = true;
                audioSourceBGM.Play();
            }
        }

    }
}
