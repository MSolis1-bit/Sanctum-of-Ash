using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public Image playerHPBar;

    private void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

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
