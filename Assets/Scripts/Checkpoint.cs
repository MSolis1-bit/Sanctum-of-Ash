using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // This line causes issues: "Object reference not set to an instance of an object"
            // GameManager.instance.playerSpawnPos.transform.position = this.transform.position;
            Debug.Log("Player Spawn Set");
        }
    }
}
