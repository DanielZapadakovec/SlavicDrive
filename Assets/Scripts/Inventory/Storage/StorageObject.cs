using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class StorageObject : MonoBehaviour
{
    [Header("Storage Settings")]
    public string storageName = "Storage";
    public int slotCount = 8;
    public List<ItemType> storedItems = new List<ItemType>();

    private bool isOpen = false;
    public UnityEvent onClose;

    public void Interact()
    {
        if (isOpen)
        {

            CloseStorage();
        }
        else
        {
            OpenStorage();
        }
    }

    public void OpenStorage()
    {
        isOpen = true;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OpenStorageUI(this);
        }
    }

    public void CloseStorage()
    {
        isOpen = false;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.CloseStorageUI();
        }
    }
}