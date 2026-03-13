using UnityEngine;

public class FallDetectorDeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            this.RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        // Damage Player?

        // Reposition Player
        GameManager.instance.player.transform.position = GameManager.instance.playerSpawnPos.transform.position;
    }
}
