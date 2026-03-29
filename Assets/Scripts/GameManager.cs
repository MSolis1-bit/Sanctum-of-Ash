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
    [SerializeField] private string firstGameplayScene = "Room1";
    [HideInInspector] public string currentScene;

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

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    void Start()
    {
        if (player != null)
        {
            UpdatePlayerUI();
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            playerHUD.SetActive(true);
        }
        else
        {
            playerHUD.SetActive(false);
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

        // Starts a brand new game from the first gameplay scene
        currentScene = firstGameplayScene;

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.NewGame();
        }

        SceneManager.LoadSceneAsync(currentScene);
    }

    public void ContinueGame()
    {
        // Makes sure the game is running normally before loading in
        StateUnpause();

        Debug.Log("ContinueGame currentScene is: " + currentScene);

        // Loads the last saved scene if one exists
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
        StateUnpause();
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentScene);
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
        this.currentScene = data.currentScene;
        Debug.Log("GameManager loaded currentScene as: " + this.currentScene);
    }

    public void SaveData(GameData data)
    {
        currentScene = SceneManager.GetActiveScene().name;
        data.currentScene = currentScene;
        Debug.Log("GameManager saved currentScene as: " + data.currentScene);
    }
}