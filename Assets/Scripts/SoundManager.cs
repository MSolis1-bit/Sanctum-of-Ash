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

    [SerializeField] Slider volumeSliderBGM;

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
        audioSourceBGM.volume = volumeSliderBGM.value;
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
