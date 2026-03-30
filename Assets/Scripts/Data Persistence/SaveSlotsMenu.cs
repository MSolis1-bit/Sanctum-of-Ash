using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Buttons: ")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup: ")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] saveSlots;

    // Determines whether we are loading a game or starting a new one
    public bool isLoadingGame = false;

    private void Awake()
    {
        // Gets all save slot UI elements under this object
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClick(SaveSlot saveSlot)
    {
        // Prevents double-clicking or spamming buttons
        DisableMenuButtons();

        if (isLoadingGame)
        {
            // Switch to the selected save profile
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

            // DO NOT LOAD SCENES HERE
            // GameManager is responsible for loading scenes correctly
            GameManager.instance.ContinueGame();
        }

        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new game with this slot will override the currently saved data. Are you sure?",

                // If player confirms overwrite
                () =>
                {
                    DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

                    // DO NOT LOAD SCENES HERE
                    // GameManager handles proper scene flow
                    GameManager.instance.NewGame();
                },

                // If player cancels
                () =>
                {
                    this.ActivateMenu();
                    isLoadingGame = true;
                }
            );
        }

        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

            // DO NOT LOAD SCENES HERE
            GameManager.instance.NewGame();
        }
    }

    public void OnDeleteClick(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this saved data?",

            // Confirm delete
            () =>
            {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileID());
                ActivateMenu();
            },

            // Cancel delete
            () =>
            {
                ActivateMenu();
                isLoadingGame = true;
            }
        );
    }

    public void ActivateMenu()
    {
        // Gets all saved profiles from disk
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // Makes sure back button works
        backButton.interactable = true;

        // Updates each save slot UI
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);

            // Displays save data info
            saveSlot.SetData(profileData);

            // Disable empty slots if trying to load
            if (profileData == null && isLoadingGame)
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
        // Disables all save slot buttons temporarily
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        backButton.interactable = false;
    }
}