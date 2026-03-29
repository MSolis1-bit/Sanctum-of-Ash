using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    [Header("Player UI: ")]
    [SerializeField] GameObject playerHUD;
    [SerializeField] Image playerHPBar;

    [Header("Scene Settings")]
    [HideInInspector] public int levelStartScene;
    [HideInInspector] public int spawnScene;

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerController playerScript;

    // For Checkpoints
    [Header("Spawn Points: ")]
    public GameObject playerSpawnPos;

    private bool isPaused = false;

    private float timeScaleOriginal;

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

        timeScaleOriginal = Time.timeScale;
   
    }

    void Start()
    {
        playerHUD = GameObject.FindWithTag("HUD");
        player = GameObject.FindWithTag("Player");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
            UpdatePlayerUI();
        }

        if (playerHUD != null)
        {
            if (SceneManager.GetActiveScene().name != null && SceneManager.GetActiveScene().name != "MainMenu")
            {
                playerHUD.SetActive(true);
            }
            else
            {
                playerHUD.SetActive(false);
            }
        }
    }

    void Update()
    {

    }

    public void UpdatePlayerUI()
    {
        if (playerHPBar == null || playerScript == null)
        {
            return;
        }

        playerHPBar.fillAmount = Mathf.Clamp01((float)playerScript.CurrentHealth / playerScript.MaxHealth);
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
        FindSceneReferences();

        if (playerScript != null)
        {
            UpdatePlayerUI();
        }

        if (playerHUD != null)
        {
            if (scene.name != "MainMenu")
            {
                playerHUD.SetActive(true);
            }
            else
            {
                playerHUD.SetActive(false);
            }
        }
    }

    private void FindSceneReferences()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }
        else
        {
            playerScript = null;
        }

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        GameObject hpBarObject = GameObject.Find("PlayerHPBarFill");
        if (hpBarObject != null)
        {
            playerHPBar = hpBarObject.GetComponent<Image>();
        }
    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void NewGame()
    {
        // Makes sure the game is running normally before loading in
        StateUnpause();

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.NewGame();
        }

        SceneManager.LoadSceneAsync(levelStartScene);
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame currentScene is: " + levelStartScene);

        if(RespawnManager.instance.HasRespawnPoint)
        {
            // If the player has a checkpoint, loads the scene with the checkpoint
            SceneManager.LoadSceneAsync(spawnScene);

            // Call the respawn manager to spawn the player at the spawn point if a spawn point has been saved
            RespawnManager.instance.Respawn(player.transform);
        }
        else
        {
            // If the player does not have a checkpoint, starts the player at the beginning of the level
            SceneManager.LoadSceneAsync(levelStartScene);
        }

        // Starts the player in a fresh state after loading
        StateUnpause();
        playerScript.ResetPlayerState();
        UpdatePlayerUI();
    }

    public void RestartLevel()
    {
        // TO DO: save player stats at the beginning of the level so they can be reset

        // Resets the players stats from data
        DataPersistenceManager.instance.LoadGame();

        // Starts the player at the beginning of the level
        SceneManager.LoadSceneAsync(levelStartScene);

        // Starts the player in a fresh state
        StateUnpause();
        playerScript.ResetPlayerState();
        UpdatePlayerUI();
    }

    public void PlayerLoses()
    {
        if (playerScript != null && playerScript.IsDead)
        {
            StatePause();
        }
    }

    public void LoadData(GameData data)
    {
        this.levelStartScene = data.levelStartScene;
    }

    public void SaveData(GameData data)
    {
        data.levelStartScene = levelStartScene;
    }
}