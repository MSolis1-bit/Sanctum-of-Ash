using UnityEngine;

// This makes sure the object has a 2D collider on it
[RequireComponent(typeof(Collider2D))]
public class RespawnPoint : MonoBehaviour
{
    // This lets the respawn point use different position sources
    private enum CoordinateSource
    {
        TransformPosition,
        ColliderPosition,
        Vector2Variable
    }

    [SerializeField] private CoordinateSource coordinateSource;
    [SerializeField] private Vector2 respawnCoordinates;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only update the respawn point when the player touches it
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        switch (coordinateSource)
        {
            default:
            case CoordinateSource.TransformPosition:
                RespawnManager.instance.SetRespawnPoint(transform.position);
                break;

            case CoordinateSource.ColliderPosition:
                RespawnManager.instance.SetRespawnPoint(GetComponent<Collider2D>().bounds.center);
                break;

            case CoordinateSource.Vector2Variable:
                RespawnManager.instance.SetRespawnPoint(respawnCoordinates);
                break;
        }

        Debug.Log("Player reached respawn point");
    }
}
