using UnityEngine;

public class InventoryDemo : MonoBehaviour
{
    public Item[] itemsToPickup;

    public void PickupItem(int item)
    {
        bool result = InventoryManager.instance.AddItem(itemsToPickup[item]);
        if(result)
        {
            Debug.Log("Item added!");
        }
        else
        {
            Debug.Log("Item not added!");
        }
    }

    public void GetSelectedItem(InventorySlot slot)
    {
        Item receivedItem =  InventoryManager.instance.GetSelectedItem(slot, false);
        if(receivedItem != null) 
        {
            Debug.Log("Item received!");
        }
        else
        {
            Debug.Log("Item not received!");
        }
    }

    public void UseSelectedItem(InventorySlot slot)
    {
        InventoryManager.instance.GetSelectedItem(slot, true);
    }
}
