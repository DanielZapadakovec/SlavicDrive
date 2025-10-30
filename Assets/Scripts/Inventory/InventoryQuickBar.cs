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
        DraggableItem itemUIScript = itemUI.GetComponent<DraggableItem>();
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
                DraggableItem draggable = child.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    Debug.Log("Si za checkom");
                    slots[i].itemType = draggable.itemType;
                    slots[i].consumableData = ItemDatabase.GetConsumableData(draggable.itemType);

                    slots[i].itemPrefabInstance = child.gameObject;
                }
            }
        }
    }
}