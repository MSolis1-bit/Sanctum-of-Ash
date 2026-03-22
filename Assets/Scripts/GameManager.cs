using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    [Header("Player UI: ")]
    [SerializeField] Image playerHPBar;

    [HideInInspector] public GameObject player;

    [HideInInspector] public PlayerController playerScript;

    private string currentScene = "";

    // For Checkpoints
    [Header("Spawn Points: ")]
    public GameObject playerSpawnPos;

    private bool isPaused = false;

    private float timeScaleOriginal;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePlayerUI()
    {
       playerHPBar.fillAmount = (float)playerScript.CurrentHealth / playerScript.MaxHealth;
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
        currentScene = "Showcase";
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

        if(currentScene == "")
        {
            SceneManager.LoadSceneAsync("Showcase");
        }
        else
        {
            SceneManager.LoadSceneAsync(currentScene);
        }
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
        currentScene = data.currentScene;
    }

    public void SaveData(GameData data)
    {
        data.currentScene = currentScene;
    }
}
