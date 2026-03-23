using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Buttons: ")]
    [SerializeField] private Button backButton;

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

        // Update the selected profile ID to be used for data persistence
        DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

        if(isLoadingGame)
        {
            // Create a new game - which will initialize our data to a clean slate
            DataPersistenceManager.instance.NewGame();
        }

        // Save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        // Load the scene - which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("Showcase");
    }

    public void OnDeleteClick(SaveSlot saveSlot)
    {
        DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileID());
        ActivateMenu();
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
