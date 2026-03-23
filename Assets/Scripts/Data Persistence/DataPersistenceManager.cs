using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileID = false;
    [SerializeField] private string testSelectedProfileID = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] bool useEncryption;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    private string selectedProfileID = "";
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Found more than one DataPersistenceManager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if(disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        this.selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileID();
        if(overrideSelectedProfileID)
        {
            this.selectedProfileID = testSelectedProfileID;
            Debug.LogWarning("Overrode selected profile ID with test ID: " + testSelectedProfileID);
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        // Update the profile to use for saving and loading
        this.selectedProfileID = newProfileID;

        // Load the game, which will use the profile, updating our game data accordingly
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Return right away if data persistence is disabled
        if(disableDataPersistence)
        {
            return;
        }

        // Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileID);

        // Start a new game if the data is null and we're configured to initialize data for debugging purposes
        if(this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // If no data can be loaded, initialize to a new game
        if(this.gameData == null)
        {
            Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
            NewGame(); 
        }

        // Push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame() 
    {
        // Return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        // If we don't have any data to save, log a warning here.
        if (this.gameData == null) 
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }

        // Pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // Timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save that data to a file using the handler
        dataHandler.Save(gameData, selectedProfileID);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
}
