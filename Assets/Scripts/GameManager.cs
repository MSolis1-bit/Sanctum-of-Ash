using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Image playerHPBar;

    public GameObject player;
    private PlayerController playerScript;

    // For Checkpoints
    public GameObject playerSpawnPos;

    private bool isPaused = false;

    float timeScaleOriginal;

    private void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        playerScript = player.GetComponent<PlayerController>();
        player = GameObject.FindWithTag("Player");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
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
}
