using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID;

    private void Start()
    {
        if (SpawnManager.nextSpawnPoint == spawnID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = transform.position;
        }
    }
}
