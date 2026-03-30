using UnityEngine;

[System.Serializable]
public class GameData
{
    // Data management
    public string timeStamp;
    public long lastUpdated;

    // Player stats
    public int maxHealth;
    public int currentHealth;
    public bool hasDash;
    public bool hasDoubleJump;

    // Level progression
    public int levelStartScene;
    public int spawnScene;
    public Vector2 respawnPoint;

    // The values defined in this constructor will be the default values
    // What the game starts with when there's no data to load
    public GameData()
    {
        this.maxHealth = 100;
        this.currentHealth = 100;
        this.hasDash = false;
        this.hasDoubleJump = false;
        this.levelStartScene = 1;
        this.spawnScene = 1;
        this.respawnPoint = Vector2.zero;
    }
}
