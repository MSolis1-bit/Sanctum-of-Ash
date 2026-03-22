using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Menu Objects: ")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuBackground;
    [SerializeField] GameObject creditsButton;
    [SerializeField] GameObject gameTitle;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject saveMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject winMenu;

    [Header("Menu Buttons: ")]
    [SerializeField] GameObject mainMenuFirstButton;
    [SerializeField] GameObject pauseFirstButton;
    [SerializeField] GameObject optionsFirstButton;

    private SaveSlotsMenu saveSlotsMenu;
    private GameObject menuActive;
    private GameObject menuPrevious;

    void Start()
    {
        saveSlotsMenu = saveMenu.GetComponentInChildren<SaveSlotsMenu>();

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenuBackground.SetActive(true);
            mainMenu.SetActive(true);
            creditsButton.SetActive(true);
            gameTitle.SetActive(true);
            menuActive = mainMenu;
            EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

            if (!DataPersistenceManager.instance.HasGameData())
            {
                Button continueButton = FindAttachedButton(menuActive.transform, "ContinueButton");
                continueButton.interactable = false;
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
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
                    menuActive = pauseMenu;
                    menuActive.SetActive(true);

                    // Clear current selected object
                    EventSystem.current.SetSelectedGameObject(pauseFirstButton);
                }
                else if(menuActive == pauseMenu)
                {
                    Resume();
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    menuActive.SetActive(false);
                    menuActive = menuPrevious;
                    menuActive.SetActive(true);
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
        if(menuActive != null && button.name != "BackButton") { menuPrevious = menuActive; }

        menuActive.SetActive(false);

        if (button.name == "CreditsButton") {menuActive = creditsMenu;}
        else if (button.name == "SaveButton" || button.name == "LoadButton") { menuActive = saveMenu; }
        else if(button.name == "NewGameButton") { saveSlotsMenu.ActivateMenu(false); }
        else if(button.name == "OptionsButton") {menuActive = optionsMenu; EventSystem.current.SetSelectedGameObject(optionsFirstButton); }
        else if(button.name == "BackButton") 
        {
            menuActive = menuPrevious; 
            if(SceneManager.GetActiveScene().name == "MainMenu")
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
            }
        }

        menuActive.SetActive(true);
    }

    public void SaveSlotsMenu()
    {

    }

    public void SFXPreview()
    {
        SoundManager.instance.PlayRandomSound();
    }
    public void NewGame()
    {
        DisableMenuButtons();
        GameManager.instance.NewGame();
    }

    public void ContinueGame()
    {
        DisableMenuButtons();
        GameManager.instance.ContinueGame();
    }

    public void QuitToMainMenu()
    {
        DisableMenuButtons();
        SceneManager.LoadScene(0);
    }

    public void ActivateLoseMenu()
    {
        menuActive = loseMenu;
        menuActive.SetActive(true);
    }

    private void DisableMenuButtons()
    {
        Button[] buttonsInMenu = menuActive.GetComponentsInChildren<Button>();
        foreach(Button button in buttonsInMenu) 
        {
            button.interactable = false;
        }
    }

    public Button FindAttachedButton(Transform parent, string name)
    {
        Transform childTransform =  parent.Find(name);
        return childTransform.GetComponent<Button>();
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
