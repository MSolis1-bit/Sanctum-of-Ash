using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnPointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Tells spawn system where player should appear
            SpawnManager.nextSpawnPoint = spawnPointID;

            // Load new scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}