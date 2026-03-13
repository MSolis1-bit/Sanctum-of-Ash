using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public Image playerHPBar;

    // For Checkpoints
    public GameObject playerSpawnPos;

    private void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdatePlayerUI()
    {
       
    }
}
