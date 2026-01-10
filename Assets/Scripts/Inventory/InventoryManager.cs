using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventoryQuickBar playerInventory;
    public StorageInventory storageUI;

    private StorageObject currentStorage;


    private void Awake()
    {
        Instance = this;
    }

    public void OpenStorageUI(StorageObject storage)
    {
        currentStorage = storage;
        storageUI.OpenStorage(storage);
    }

    public void CloseStorageUI()
    {
        if (currentStorage != null)
        {
            storageUI.CloseStorage();
            currentStorage = null;
        }
    }
    public void MoveItem(ClickableItem item, bool fromStorage)
    {
        if (fromStorage)
        {
            if (playerInventory.HasFreeSlot())
            {
                playerInventory.AddItem(item.itemType);
                storageUI.RemoveItem(item.itemType);
            }
            else
            {
                Debug.Log("Player inventory full!");
            }
        }
        else
        {
            if (storageUI.HasFreeSlot() && storageUI.currentStorage.CanAcceptItem(item.itemType))
            {
                storageUI.AddItem(item.itemType);
                playerInventory.RemoveItem(item.itemType);
            }
            else
            {
                Debug.Log("Storage full!");
            }
        }
    }
}
