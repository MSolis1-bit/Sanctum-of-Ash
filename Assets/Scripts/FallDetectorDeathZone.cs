using UnityEngine;

public class FallDetectorDeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player triggered respawn pit");
            this.RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        // Damage Player?

        // Reposition Player/Respawn Player
        // GameManager.instance.player.SpawnPlayer();
        Debug.Log("Player position reset");
    }
}
