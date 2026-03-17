using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
