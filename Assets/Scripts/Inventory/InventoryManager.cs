using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventoryQuickBar playerInventory;
    public StorageInventory storageInventory;

    private void Awake()
    {
        Instance = this;
    }
    public void MoveItem(ClickableItem item, bool fromStorage)
    {
        if (fromStorage)
        {
            if (playerInventory.HasFreeSlot())
            {
                playerInventory.AddItem(item.itemType);
                storageInventory.RemoveItem(item.itemType);
            }
            else
            {
                Debug.Log("Player inventory full!");
            }
        }
        else
        {
            if (storageInventory.HasFreeSlot())
            {
                storageInventory.AddItem(item.itemType);
                playerInventory.RemoveItem(item.itemType);
            }
            else
            {
                Debug.Log("Storage full!");
            }
        }
    }
}
