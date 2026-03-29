using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour, IDataPersistence
{
    // Lets other scripts access this manager
    public static RespawnManager instance;

    // Stores the current respawn point
    private Vector2 respawnPoint;

    // Keeps track of whether a respawn point has been saved yet
    private bool hasRespawnPoint = false;

    public bool HasRespawnPoint => hasRespawnPoint;

    private void Awake()
    {
        // Makes sure only one RespawnManager exists
        if (instance == null)
        {
            instance = this;
            Debug.Log("RespawnManager set to: " + gameObject.name + " | ID: " + GetInstanceID());
        }
        else
        {
            Debug.LogWarning("Duplicate RespawnManager found on: " + gameObject.name + " | ID: " + GetInstanceID());
            Destroy(gameObject);
            return;
        }
    }

    // Saves a new respawn point
    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        hasRespawnPoint = true;

        Debug.Log("Player Spawn Set on: " + gameObject.name +
                  " | ID: " + GetInstanceID() +
                  " | Point: " + respawnPoint);

        // Sets variables for the game manager to know where to load the player on load games
        GameManager.instance.spawnScene = SceneManager.GetActiveScene().buildIndex;

        // Save the game
        DataPersistenceManager.instance.SaveGame();
    }

    // Sends the player back to the saved spawn point
    public bool Respawn(Transform playerTransform)
    {
        Debug.Log("Respawn called on: " + gameObject.name +
                  " | ID: " + GetInstanceID() +
                  " | hasRespawnPoint: " + hasRespawnPoint);

        // Stops the game from trying to respawn before a checkpoint is reached
        if (!hasRespawnPoint)
        {
            Debug.LogWarning("No respawn point has been set yet");
            return false;
        }

        // Moves the player back to the saved position
        playerTransform.position = respawnPoint;
        Debug.Log("Respawning player to: " + respawnPoint);

        return true;
    }

    public void LoadData(GameData data)
    {
        this.respawnPoint = data.respawnPoint;
    }

    public void SaveData(GameData data)
    {
        data.respawnPoint = this.respawnPoint;
    }
}