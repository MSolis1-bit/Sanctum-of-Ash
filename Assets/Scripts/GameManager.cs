using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player UI: ")]
    [SerializeField] Image playerHPBar;

    [HideInInspector] public GameObject player;

    [HideInInspector] public PlayerController playerScript;

    // For Checkpoints
    public GameObject playerSpawnPos;

    private bool isPaused = false;

    private float timeScaleOriginal;

    private void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }
    void Start()
    {
        UpdatePlayerUI();
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

    }

    public void PlayerLoses()
    {
        if(playerScript.IsDead == true)
        {
            StatePause();
        }
    }
}
