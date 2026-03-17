using UnityEngine;

public class FallDetectorDeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player entered death zone");
        RespawnPlayer(collision.transform);
    }

    // Sends the player back to the saved respawn point
    private void RespawnPlayer(Transform playerTransform)
    {
        RespawnManager.instance.Respawn(playerTransform);
        Debug.Log("Player position reset");
    }
}
