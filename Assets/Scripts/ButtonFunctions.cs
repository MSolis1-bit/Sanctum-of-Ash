using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;

    private GameObject menuActive;
    private GameObject menuPrevious;

    void Start()
    {
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuActive = mainMenu;
        }
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if(menuActive == null)
                {
                    GameManager.instance.StatePause();
                    menuActive = menuPause;
                    menuActive.SetActive(true);
                }
                else if(menuActive == menuPause)
                {
                    Resume();
                }
            }
        }
    }

    public void Resume()
    {
        GameManager.instance.StateUnpause();
        menuActive.SetActive(false);
        menuActive = null;
    }

    // takes in the name of the button to determine the menu to toggle
    public void ToggleMenu(Button button)
    {
        if(menuActive != null) { menuPrevious = menuActive; }

        menuActive.SetActive(false);

        if (button.name == "CreditsButton") {menuActive = creditsMenu;}
        else if(button.name == "OptionsButton") {menuActive = optionsMenu;}
        else if(button.name == "BackButton") {menuActive = menuPrevious;}

        menuActive.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
