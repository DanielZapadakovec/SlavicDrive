using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StorageObject : MonoBehaviour
{
    [Header("Storage Settings")]
    public string storageName = "Storage";
    public int slotCount = 8;
    public List<ItemType> storedItems = new List<ItemType>();

    [Header("Item Filter")]
    public bool useItemFilter = false;
    public List<ItemType> allowedItems = new List<ItemType>();

    private bool isOpen = false;
    public UnityEvent onClose;

    // =========================
    // INTERACT
    // =========================
    public void Interact()
    {
        if (isOpen)
            CloseStorage();
        else
            OpenStorage();
    }

    // =========================
    // OPEN / CLOSE
    // =========================
    public void OpenStorage()
    {
        isOpen = true;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OpenStorageUI(this);
    }

    public void CloseStorage()
    {
        isOpen = false;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.CloseStorageUI();
    }

    // =========================
    // ITEM FILTER LOGIC
    // =========================
    public bool CanStoreItem(ItemType type)
    {
        if (!useItemFilter)
            return true;

        return allowedItems.Contains(type);
    }

    public bool TryAddItem(ItemType type)
    {
        if (storedItems.Count >= slotCount)
            return false;

        if (!CanStoreItem(type))
            return false;

        storedItems.Add(type);
        return true;
    }

    // =========================
    // HELPERS
    // =========================
    public int CountItem(ItemType type)
    {
        int count = 0;
        foreach (var item in storedItems)
        {
            if (item == type)
                count++;
        }
        return count;
    }

    public void RemoveItems(ItemType type, int amount)
    {
        for (int i = storedItems.Count - 1; i >= 0 && amount > 0; i--)
        {
            if (storedItems[i] == type)
            {
                storedItems.RemoveAt(i);
                amount--;
            }
        }
    }
    public bool CanAcceptItem(ItemType type)
    {
        if (!useItemFilter)
            return true;

        return allowedItems.Contains(type);
    }
}
