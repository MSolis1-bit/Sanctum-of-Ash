using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;

    private void Start()
    {
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuActive = mainMenu;
        }
    }

    // takes in the name of the button to determine the menu to toggle
    public void ToggleMenu(Button button)
    {
        menuActive.SetActive(false);

        if (button.name == "CreditsButton")
        {
            menuActive = creditsMenu;
        }
        else if(button.name == "OptionsButton")
        {
            menuActive = optionsMenu;
        }
        else if(button.name == "BackButton")
        {
            menuActive = mainMenu;
        }

        menuActive.SetActive(true);
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
