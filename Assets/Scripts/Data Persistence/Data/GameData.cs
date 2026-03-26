using UnityEngine;

[System.Serializable]
public class GameData
{
    public string timeStamp;
    public long lastUpdated;

    public int maxHealth;
    public bool hasDash;
    public bool hasDoubleJump;
    public int currentScene;

    // The values defined in this constructor will be the default values
    // What the game starts with when there's no data to load
    public GameData()
    {
        this.maxHealth = 100;
        this.hasDash = false;
        this.hasDoubleJump = false;
        this.currentScene = 1;
    }
}
