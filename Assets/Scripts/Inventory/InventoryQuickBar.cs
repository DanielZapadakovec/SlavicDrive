using UnityEngine;
using UnityEngine.UI;

public class InventoryQuickBar : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType itemType = ItemType.None;
        public GameObject itemPrefabInstance;
        public ConsumableData consumableData;
    }

    public InventorySlot[] slots = new InventorySlot[4];
    public Transform[] slotParents;
    public int activeSlot = 0;


    [Header("Prefabs")]
    public GameObject itemUIPrefab;

    [Header("Player Hand")]
    public Transform handTransform;
    public GameObject currentHeldItem;

    [Header("UI Highlight")]
    public Color normalColor = new Color(1, 1, 1, 0.5f);
    public Color activeColor = Color.white;
    public UIManager uiManager;

    [Header("Audio")]
    public AudioSource audioProps;
    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
            ClearSlot(i);
        UpdateUIHighlight();
    }

    private void Update()
    {
        HandleSlotSwitch();
        CheckIfItemStillExists();
        ConsumeActiveItem();
    }

    private void HandleSlotSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveSlot(3);
    }

    public void SetActiveSlot(int index)
    {
        activeSlot = index;
        UpdateUIHighlight();
        UpdateHeldItem();
    }

    private void UpdateUIHighlight()
    {
        for (int i = 0; i < slotParents.Length; i++)
        {
            Image img = slotParents[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == activeSlot) ? activeColor : normalColor;
        }
    }

    public void SetSlot(int index, ItemType type, Sprite icon, ConsumableData consumable)
    {
        ClearSlot(index);

        slots[index].itemType = type;
        slots[index].consumableData = ItemDatabase.GetConsumableData(type);

        GameObject itemUI = Instantiate(itemUIPrefab, slotParents[index]);
        ClickableItem itemUIScript = itemUI.GetComponent<ClickableItem>();
        itemUIScript.itemType = type;
        itemUIScript.iconSlot.sprite = icon;

        slots[index].itemPrefabInstance = itemUI;

        if (index == activeSlot)
            UpdateHeldItem();
    }

    public void ClearSlot(int index)
    {
        slots[index].itemType = ItemType.None;
        slots[index].consumableData = null;

        if (slots[index].itemPrefabInstance != null)
            Destroy(slots[index].itemPrefabInstance);

        slots[index].itemPrefabInstance = null;

        if (index == activeSlot)
            UpdateHeldItem();
    }

    private void UpdateHeldItem()
    {
        if (currentHeldItem != null)
            Destroy(currentHeldItem);

        var slot = slots[activeSlot];
        if (slot.itemType == ItemType.None) return;

        GameObject prefab = ItemDatabase.GetPrefab(slot.itemType);
        if (prefab != null)
        {
            currentHeldItem = Instantiate(prefab, handTransform);
            currentHeldItem.transform.localPosition = Vector3.zero;
            currentHeldItem.transform.localRotation = Quaternion.identity;

            if (currentHeldItem.TryGetComponent(out Rigidbody rb))
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            if (currentHeldItem.TryGetComponent(out Collider col))
                col.enabled = false;
        }
    }
    private void CheckIfItemStillExists()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slotParents[i].childCount == 0)
            {
                slots[i].itemType = ItemType.None;
                slots[i].consumableData = null;
                slots[i].itemPrefabInstance = null;
            }
            else
            {
                Transform child = slotParents[i].GetChild(0);
                ClickableItem draggable = child.GetComponent<ClickableItem>();
                if (draggable != null)
                {
                    slots[i].itemType = draggable.itemType;
                    slots[i].consumableData = ItemDatabase.GetConsumableData(draggable.itemType);

                    slots[i].itemPrefabInstance = child.gameObject;
                }
            }
        }
    }
    public bool TryPickUpItem(ItemID item, Transform dropPoint, Camera cam)
    {
        int slot = activeSlot;

        if (slots[slot].itemType != ItemType.None)
        {
            GameObject oldObj = ItemDatabase.SpawnItem(slots[slot].itemType, dropPoint.position);
            if (oldObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.AddForce(cam.transform.forward * 2f, ForceMode.Impulse);
        }

        ItemType type = item.itemType;
        Sprite icon = ItemDatabase.GetIcon(type);
        ConsumableData consumable = ItemDatabase.GetConsumableData(type);

        SetSlot(slot, type, icon, consumable);
        Destroy(item.gameObject);

        return true;
    }

    public void DropActiveItem(Transform dropPoint, Camera cam)
    {
        int slot = activeSlot;
        if (slots[slot].itemType == ItemType.None) return;

        GameObject dropObj = ItemDatabase.SpawnItem(slots[slot].itemType, dropPoint.position);
        if (dropObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb.AddForce(cam.transform.forward * 2f, ForceMode.Impulse);

        ClearSlot(slot);
    }

    public void ConsumeActiveItem()
    {       
        int slotIndex = activeSlot;
        var data = slots[slotIndex].consumableData;
        if (data == null || (!data.isDrink && !data.isFood)) return;

        bool canConsume = true;

        PlayerStatsSystem stats = FindAnyObjectByType<PlayerStatsSystem>();
        if (stats == null) return;
        uiManager.ShowEKeyBind(canConsume);
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (data.isFood) stats.AddHunger(data.foodAmount);
            if (data.isDrink) stats.AddThirst(data.drinkAmount);
            if (data.sound != null && !audioProps.isPlaying)
            {
                audioProps.PlayOneShot(data.sound);
            }
            
            ClearSlot(slotIndex);
            canConsume = false;
        }
    }
    public bool HasFreeSlot()
    {
        foreach (var s in slotParents)
            if (s.childCount == 0) return true;
        return false;
    }

    public void AddItem(ItemType type)
    {
        foreach (var s in slotParents)
        {
            if (s.childCount == 0)
            {
                var ui = Instantiate(itemUIPrefab, s);
                var clickable = ui.GetComponent<ClickableItem>();
                clickable.itemType = type;
                clickable.isInStorage = false;
                clickable.iconSlot.sprite = ItemDatabase.GetIcon(type);
                return;
            }
        }
    }

    public void RemoveItem(ItemType type)
    {
        foreach (var s in slotParents)
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
}