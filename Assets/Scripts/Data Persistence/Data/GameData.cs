using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    //public Vector2 playerPosition;
    public int maxHealth;
    public bool hasDash;
    public bool hasDoubleJump;
    public string currentScene;

    // The values defined in this constructor will be the default values
    // What the game starts with when there's no data to load
    public GameData()
    {
        this.currentScene = "";
        this.maxHealth = 5;
        this.hasDash = false;
        this.hasDoubleJump = false;
    }
}
