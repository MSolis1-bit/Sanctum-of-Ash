using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isAdditive;
    [SerializeField] private string spawnPointID;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            isTransitioning = true;

            // Tells spawn system where player should appear
            SpawnManager.nextSpawnPoint = spawnPointID;

            // If using additive loading, only load the scene if it is not already loaded
            if (isAdditive)
            {
                Scene targetScene = SceneManager.GetSceneByName(sceneToLoad);

                if (!targetScene.isLoaded)
                {
                    SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
                }
            }
            else
            {
                // For normal loading, only load if it is not already the active scene
                if (sceneToLoad != SceneManager.GetActiveScene().name)
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTransitioning = false;
        }
    }
}