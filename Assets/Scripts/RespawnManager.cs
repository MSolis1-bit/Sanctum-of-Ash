using UnityEngine;

public class RespawnManager : MonoBehaviour, IDataPersistence
{
    // Singleton reference so other scripts can access this easily
    public static RespawnManager instance;

    // Stores the current respawn position during gameplay
    private Vector2 respawnPoint;

    // Tracks whether a checkpoint has been set yet
    private bool hasRespawnPoint = false;

    public bool HasRespawnPoint => hasRespawnPoint;

    private void Awake()
    {
        // Ensures only ONE RespawnManager exists
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

    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        // Saves a checkpoint position during gameplay
        respawnPoint = newRespawnPoint;
        hasRespawnPoint = true;

        Debug.Log("Player Spawn Set on: " + gameObject.name +
                  " | ID: " + GetInstanceID() +
                  " | Point: " + respawnPoint);

        // Saves the game after updating checkpoint
        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
        }
    }

    public bool Respawn(Transform playerTransform)
    {
        Debug.Log("Respawn called on: " + gameObject.name +
                  " | ID: " + GetInstanceID() +
                  " | hasRespawnPoint: " + hasRespawnPoint);

        // Prevents respawn if no checkpoint has been set
        if (!hasRespawnPoint)
        {
            Debug.LogWarning("No respawn point has been set yet");
            return false;
        }

        // Moves player to checkpoint position
        playerTransform.position = respawnPoint;

        Debug.Log("Respawning player to: " + respawnPoint);

        return true;
    }

    public void LoadData(GameData data)
    {
        // DO NOT LOAD RESPAWN FROM SAVE FILE
        // Respawn is handled during gameplay only
    }

    public void SaveData(GameData data)
    {
        // DO NOT SAVE RESPAWN TO SAVE FILE
        // This prevents conflicts with scene loading system
    }
}