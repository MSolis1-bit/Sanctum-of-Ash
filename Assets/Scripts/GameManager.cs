using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenuButtons;
    [SerializeField] GameObject optionsMenu;

    public GameObject player;
    public Image playerHPBar;

    private void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuActive = mainMenuButtons;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdatePlayerUI()
    {
       
    }

    public void ToggleOptionsMenu()
    {
        menuActive.SetActive(false);
        menuActive = optionsMenu;
        menuActive.SetActive(true);
    }

    public void BackButton()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuActive.SetActive(false);
            menuActive = mainMenuButtons;
            menuActive.SetActive(true);
        }
    }
}
