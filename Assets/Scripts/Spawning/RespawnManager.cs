using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    // Lets other scripts access this manager
    public static RespawnManager instance;

    // Stores the current respawn point
    private Vector2 respawnPoint;

    // Keeps track of whether a respawn point has been set yet
    private bool hasRespawnPoint;

    private void Awake()
    {
        // Makes sure there is only one RespawnManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Changes the current respawn point
    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        hasRespawnPoint = true;

        Debug.Log("Respawn point set to: " + respawnPoint);
    }

    // Returns the current respawn point
    public Vector2 GetRespawnPoint()
    {
        return respawnPoint;
    }

    // Moves the player back to the saved respawn point
    public void Respawn(Transform respawnable)
    {
        // Prevents errors if no respawn point was touched yet
        if(!hasRespawnPoint)
        {
            Debug.LogWarning("No respawn point has been set yet.");
            return;
        }

        respawnable.position = respawnPoint;
        Debug.Log("Player respawned at: " + respawnPoint);
    }
}
