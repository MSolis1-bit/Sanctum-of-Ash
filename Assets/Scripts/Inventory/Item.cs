using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("Gameplay")]
    public Sprite image;
    public ItemType type;
    public ActionType actionType;

    [Header("UI")]
    public bool stackable = true;
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Key
}

public enum ActionType
{
    None,
    Consumable,
    Usable,
}

