using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Buttons: ")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup: ")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] saveSlots;

    public bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClick(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableMenuButtons();

        // Case - loading game
        if(isLoadingGame)
        {
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            SaveGameAndLoadScene();
        }
        // Case - new game, but the save slot has data
        else if(saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new game with this slot will override the currently saved data. Are you sure?",
                // Function to execute if we select 'confirm'
                () =>
                {
                    DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
                    DataPersistenceManager.instance.NewGame();
                    if (GameManager.instance.IsPaused) { GameManager.instance.ResetTimeScale(); }
                    SaveGameAndLoadScene();
                },
                () =>
                // Function to execute if we select 'cancel'
                {
                    this.ActivateMenu();
                    isLoadingGame = true;
                }
                );
        }
        else
        {
            // Case - new game, and the save slot has no data
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            DataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    private void SaveGameAndLoadScene()
    {
        // Save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        // Load the scene - which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("Showcase");
    }

    public void OnDeleteClick(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            // Function to execute if we select 'confirm'
            () =>
            {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileID());
                ActivateMenu();
            },
            // Function to execute if we select 'cancel'
            () =>
            {
                ActivateMenu();
                isLoadingGame = true;
            }
            );
    }

    private void Update()
    {
        //if(this.gameObject.activeSelf)
        //{
        //    ActivateMenu();
        //}
    }

    public void ActivateMenu()
    {
        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // Ensure the back button is enabled when we activate the menu
        backButton.interactable = true;

        // Loop through each save slot in the UI and get the content appropriately
        foreach(SaveSlot saveSlot in saveSlots) 
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);

            if(profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    private void DisableMenuButtons()
    {
        foreach(SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}
