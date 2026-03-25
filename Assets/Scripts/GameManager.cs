using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    [Header("Player UI: ")]
    [SerializeField] GameObject playerHUD;
    [SerializeField] Image playerHPBar;

    [HideInInspector] public GameObject player;

    [HideInInspector] public PlayerController playerScript;

    public int currentScene = 2;

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
        if(player != null )
        {
            playerScript = player.GetComponent<PlayerController>();
        }

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }
    void Start()
    {
        if(player != null)
        {
            UpdatePlayerUI();
        }

        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            playerHUD.SetActive(true);
        }
        else
        {
            playerHUD.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePlayerUI()
    {
        Debug.Log("UI reading from object: " + playerScript.gameObject.name + " | HP: " + playerScript.CurrentHealth + " / " + playerScript.MaxHealth);
        if (playerScript == null || playerHPBar == null)
        {
            FindSceneReferences();
        }

        if (playerScript != null && playerHPBar != null)
        {
            playerHPBar.fillAmount = (float)playerScript.CurrentHealth / playerScript.MaxHealth;
            Debug.Log("HP Bar Updated: " + playerScript.CurrentHealth + " / " + playerScript.MaxHealth);
        }
        else
        {
            Debug.LogWarning("Player UI could not update because playerScript or playerHPBar is missing.");
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
        FindSceneReferences();
        UpdatePlayerUI();
    }

    private void FindSceneReferences()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
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
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void NewGame()
    {
        // Create a new game - which will initialize our game data
        DataPersistenceManager.instance.NewGame();

        // Load the gameplay scene - which will in turn save the game because of
        // OnSceneUnloaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync(currentScene);
    }

    public void ContinueGame()
    {
        // Load the next scene - which will in turn load the game because of

        // Save the game any time before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadSceneAsync(currentScene);
    }

    public void PlayerLoses()
    {
        if(playerScript.IsDead == true)
        {
            StatePause();
        }
    }

    public void LoadData(GameData data)
    {
        this.currentScene = data.currentScene;
    }

    public void SaveData(GameData data)
    {
        data.currentScene = currentScene;
    }
}
