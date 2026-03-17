using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Image playerHPBar;

    public GameObject player;
    private PlayerController playerScript;

    // This is the object in the scene that marks the starting spawn position
    public GameObject playerSpawnPos;

    private bool isPaused = false;
    float timeScaleOriginal;

    private void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }

        // Moves the player to the starting spawn point when the scene begins
        if (player != null && playerSpawnPos != null)
        {
            player.transform.position = playerSpawnPos.transform.position;
        }
    }

    private void Start()
    {
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI()
    {
        if (playerScript != null && playerHPBar != null)
        {
            playerHPBar.fillAmount = (float)playerScript.CurrentHealth / playerScript.MaxHealth;
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
    }
}