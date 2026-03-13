using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // This variable must be replaced by the player's spawn position:
    // GameManager.instance.playerSpawnPos.transform.position
    private Vector2 spawn_pos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            spawn_pos = transform.position;
            Debug.Log("Player Spawn Set");
        }
    }
}
