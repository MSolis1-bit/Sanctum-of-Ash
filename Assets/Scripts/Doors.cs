using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isAdditive;

    [SerializeField] private string spawnPointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Tells spawn system where player should appear
            SpawnManager.nextSpawnPoint = spawnPointID;

            // Check if the scene isn't already active
            if (sceneToLoad != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                // Load new scene
                if (isAdditive)
                    SceneManager.LoadScene(sceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}