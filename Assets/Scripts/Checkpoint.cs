using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            Debug.Log("Player Spawn Set");
        }
    }
}
