using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public int maxHealth;
    public bool hasDash;
    public bool hasDoubleJump;
    // The values defined in this constructor will be the default values
    // What the game starts with when there's no data to load

    public GameData()
    {
        playerPosition = Vector3.zero;
        this.maxHealth = 5;
        this.hasDash = false;
        this.hasDoubleJump = false;
    }
}
