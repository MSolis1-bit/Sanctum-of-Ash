using Unity.VisualScripting;
using UnityEngine;

public class FallDetectorDeathZone : MonoBehaviour
{
    // This function runs when something enters the trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only run the respawn logic if the object is the player
        if (!collision.CompareTag("Player"))
        {
            // Destroy the object that isn't the player
            Destroy(collision.gameObject);
            return;
        }

        // Print a message so we know the player touched the death zone
        Debug.Log("Player entered death zone");

        // Send the player to the respawn function
        RespawnPlayer(collision.transform);
    }

    // This function handles moving the player back to the saved respawn point
    private void RespawnPlayer(Transform playerTransform)
    {
        // Check to make sure the RespawnManager exists in the scene
        if (RespawnManager.instance == null)
        {
            Debug.LogError("RespawnManager.instance is null");
            return;
        }

        // Try to move the player back to the saved checkpoint
        bool didRespawn = RespawnManager.instance.Respawn(playerTransform);

        // Only print this if the respawn actually happened
        if (didRespawn)
        {
            Debug.Log("Player position reset");
        }
    }
}