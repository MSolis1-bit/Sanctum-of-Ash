using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;

    private static Checkpoint currentCheckpoint;

    private void Start()
    {
        // Sets the checkpoint to its starting visual state
        SetInactiveVisual();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only let the player activate the checkpoint
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // Make sure the RespawnManager exists before trying to use it
        if (RespawnManager.instance == null)
        {
            Debug.LogError("RespawnManager.instance is null");
            return;
        }

        // Turn off the old checkpoint if there was one
        if (currentCheckpoint != null && currentCheckpoint != this)
        {
            currentCheckpoint.SetInactiveVisual();
        }

        // Save this checkpoint position as the new respawn point
        RespawnManager.instance.SetRespawnPoint(transform.position);

        // Remember this as the active checkpoint
        currentCheckpoint = this;

        // Show that this checkpoint is now active
        SetActiveVisual();

        Debug.Log("Checkpoint activated");
    }

    private void SetActiveVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = activeColor;
        }
    }

    private void SetInactiveVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }
    }
}