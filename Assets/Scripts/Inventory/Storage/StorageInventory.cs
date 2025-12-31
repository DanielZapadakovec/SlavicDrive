using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageInventory : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform slotParent;
    public GameObject itemUIPrefab;
    public GameObject slotUIPrefab;

    private StorageObject currentStorage;
    private List<Transform> slots = new List<Transform>();
    public Animator storageAnimator;
    public static bool isOpen;

    public void Update()
    {
     if (isOpen)
     {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseStorage();
        }
     }
    }
    public void OpenStorage(StorageObject storage)
    {
        ClearUI();
        currentStorage = storage;

        for (int i = 0; i < storage.slotCount; i++)
        {
            var slot = Instantiate(slotUIPrefab, slotParent).transform;
            slots.Add(slot);
        }

        foreach (var itemType in storage.storedItems)
        {
            AddItem(itemType);
        }
        PlayerController.SwitchingCameraMovement();
        storageAnimator.SetBool("isOpen", true);
        isOpen = true;
    }

    public void CloseStorage()
    {
        if (currentStorage != null)
        {

            currentStorage.storedItems.Clear();

            foreach (var s in slots)
            {
                if (s.childCount > 0)
                {
                    var clickable = s.GetChild(0).GetComponent<ClickableItem>();
                    currentStorage.storedItems.Add(clickable.itemType);
                }
            }
        }
        if (currentStorage.onClose != null)
        {
            currentStorage.onClose.Invoke();
        }
        storageAnimator.SetBool("isOpen", false);
        isOpen = false;
        PlayerController.SwitchingCameraMovement();
        ClearUI();
    }

    private void ClearUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        slots.Clear();
    }

    public void AddItem(ItemType type)
    {
        foreach (var s in slots)
        {
            if (s.childCount == 0)
            {
                var ui = Instantiate(itemUIPrefab, s);
                var clickable = ui.GetComponent<ClickableItem>();
                clickable.itemType = type;
                clickable.isInStorage = true;
                clickable.iconSlot.sprite = ItemDatabase.GetIcon(type);
                return;
            }
        }
    }
    public void RemoveItem(ItemType type)
    {
        foreach (var s in slots)
        {
            if (s.childCount > 0)
            {
                var item = s.GetChild(0).GetComponent<ClickableItem>();
                if (item.itemType == type)
                {
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }

    public bool HasFreeSlot()
    {
        foreach (var s in slots)
            if (s.childCount == 0) return true;
        return false;
    }
}
