using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    [Header("Player UI: ")]
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private Image playerHPBar;

    [Header("Scene Settings")]
    // DO NOT CHANGE THIS TO INT OR REMOVE
    // This stores the FIRST gameplay scene (used only for New Game or fallback)
    [SerializeField] private string firstGameplayScene = "Room1";

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerController playerScript;

    // DO NOT MODIFY TYPE OR REMOVE
    // This stores the LAST saved scene for Continue Game
    [SerializeField] private string currentScene = "";

    [Header("Spawn Points: ")]
    public GameObject playerSpawnPos;

    private bool isPaused = false;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Finds player + UI when the game first starts
        FindSceneReferences();

        if (playerScript != null)
        {
            UpdatePlayerUI();
        }

        // Shows HUD only if NOT in main menu
        if (playerHUD != null)
        {
            playerHUD.SetActive(SceneManager.GetActiveScene().name != "MainMenu");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Refreshes references after scene change
        FindSceneReferences();

        if (playerScript != null)
        {
            UpdatePlayerUI();
        }

        if (playerHUD != null)
        {
            playerHUD.SetActive(scene.name != "MainMenu");
        }
    }

    private void FindSceneReferences()
    {
        // Finds player in scene
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }
        else
        {
            playerScript = null;
        }

        // Finds spawn point
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        // Finds health bar UI
        GameObject hpBarObject = GameObject.Find("PlayerHPBarFill");
        if (hpBarObject != null)
        {
            playerHPBar = hpBarObject.GetComponent<Image>();
        }

        // Finds HUD
        GameObject hudObject = GameObject.FindWithTag("HUD");
        if (hudObject != null)
        {
            playerHUD = hudObject;
        }
    }

    public void UpdatePlayerUI()
    {
        // Updates health bar based on actual player health
        if (playerHPBar == null || playerScript == null)
        {
            return;
        }

        playerHPBar.fillAmount = Mathf.Clamp01((float)playerScript.CurrentHealth / playerScript.MaxHealth);
    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Shows mouse cursor for menu navigation
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        // ?? DO NOT REMOVE — THIS FIXES "PLAYER CANNOT MOVE" BUG
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void NewGame()
    {
        // Ensures game is not frozen before starting
        StateUnpause();

        // Starts fresh game at first scene
        currentScene = firstGameplayScene;

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.NewGame();
        }

        SceneManager.LoadSceneAsync(currentScene);
    }

    public void ContinueGame()
    {
        // CRITICAL FIX — DO NOT REMOVE
        // Ensures game is not frozen from pause
        StateUnpause();

        // CRITICAL FIX — LOAD SAVE DATA BEFORE USING currentScene
        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.LoadGame();
        }

        // Loads last saved scene, or fallback if empty
        if (string.IsNullOrEmpty(currentScene))
        {
            SceneManager.LoadSceneAsync(firstGameplayScene);
        }
        else
        {
            SceneManager.LoadSceneAsync(currentScene);
        }
    }

    public void RestartLevel()
    {
        // Reloads current scene cleanly
        StateUnpause();

        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentScene);
    }

    public void PlayerLoses()
    {
        // Pauses game when player dies
        if (playerScript != null && playerScript.IsDead)
        {
            StatePause();
        }
    }

    public void LoadData(GameData data)
    {
        // Loads saved scene name
        this.currentScene = data.currentScene;
    }

    public void SaveData(GameData data)
    {
        // DO NOT CHANGE — THIS IS REQUIRED FOR CONTINUE TO WORK
        // Saves the CURRENT scene name before quitting or switching scenes
        currentScene = SceneManager.GetActiveScene().name;
        data.currentScene = currentScene;
    }
}