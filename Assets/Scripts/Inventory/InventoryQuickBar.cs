using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryQuickBar : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType itemType = ItemType.None;
        public GameObject itemPrefabInstance;
        public ConsumableData consumableData;
    }

    [Header("Slots")]
    public InventorySlot[] slots = new InventorySlot[4];
    public Transform[] slotParents;
    public int activeSlot = 0;

    [Header("Input Actions (NEW INPUT SYSTEM)")]
    public InputActionReference scrollSlotAction;   // Mouse wheel
    public InputActionReference nextSlotAction;     // RB
    public InputActionReference prevSlotAction;     // LB
    public InputActionReference consumeAction;      // E / A

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

    public bool canConsume;

    #region Unity Lifecycle

    private void OnEnable()
    {
        scrollSlotAction.action.Enable();
        nextSlotAction.action.Enable();
        prevSlotAction.action.Enable();
        consumeAction.action.Enable();

        scrollSlotAction.action.performed += OnScroll;
        nextSlotAction.action.performed += _ => ChangeSlot(1);
        prevSlotAction.action.performed += _ => ChangeSlot(-1);
    }

    private void OnDisable()
    {
        scrollSlotAction.action.performed -= OnScroll;

        scrollSlotAction.action.Disable();
        nextSlotAction.action.Disable();
        prevSlotAction.action.Disable();
        consumeAction.action.Disable();
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
            ClearSlot(i);

        UpdateUIHighlight();
        UpdateHeldItem();
    }

    private void Update()
    {
        HandleKeyboardFallback();
        CheckIfItemStillExists();
        if (canConsume) { ConsumeActiveItem(); }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                Debug.Log("RB pressed");

            if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                Debug.Log("LB pressed");
        }
    
}

    #endregion

    #region Slot Switching

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;

        if (scroll > 0f)
            ChangeSlot(1);
        else if (scroll < 0f)
            ChangeSlot(-1);
    }

    private void HandleKeyboardFallback()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetActiveSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetActiveSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetActiveSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetActiveSlot(3);
    }

    private void ChangeSlot(int direction)
    {
        activeSlot = (activeSlot + direction + slots.Length) % slots.Length;
        UpdateUIHighlight();
        UpdateHeldItem();
    }

    public void SetActiveSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        activeSlot = index;
        UpdateUIHighlight();
        UpdateHeldItem();
    }

    #endregion

    #region UI

    private void UpdateUIHighlight()
    {
        for (int i = 0; i < slotParents.Length; i++)
        {
            Image img = slotParents[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == activeSlot) ? activeColor : normalColor;
            slotParents[i].transform.localScale =
            (i == activeSlot) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    #endregion

    #region Held Item

    private void UpdateHeldItem()
    {
        if (currentHeldItem != null)
            Destroy(currentHeldItem);

        InventorySlot slot = slots[activeSlot];
        if (slot.itemType == ItemType.None)
            return;

        GameObject prefab = ItemDatabase.GetPrefab(slot.itemType);
        if (prefab == null)
            return;

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

    #endregion

    #region Inventory Logic

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
        ItemType newType = item.itemType;
        Sprite icon = ItemDatabase.GetIcon(newType);
        ConsumableData consumable = ItemDatabase.GetConsumableData(newType);

        // 1️⃣ AKTÍVNY SLOT JE PRÁZDNY → DÁME TAM
        if (slots[activeSlot].itemType == ItemType.None)
        {
            SetSlot(activeSlot, newType, icon, consumable);
            Destroy(item.gameObject);
            return true;
        }

        // 2️⃣ HĽADÁME VOĽNÝ SLOT OKREM AKTÍVNEHO
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == activeSlot)
                continue;

            if (slots[i].itemType == ItemType.None)
            {
                SetSlot(i, newType, icon, consumable);
                Destroy(item.gameObject);
                return true;
            }
        }

        // 3️⃣ INVENTÁR PLNÝ → VYMEŇ AKTÍVNY SLOT
        ItemType oldType = slots[activeSlot].itemType;

        GameObject oldObj = ItemDatabase.SpawnItem(
            oldType,
            dropPoint.position
        );

        if (oldObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb.AddForce(cam.transform.forward * 2f, ForceMode.Impulse);

        SetSlot(activeSlot, newType, icon, consumable);
        Destroy(item.gameObject);

        return true;
    }

    private int GetFirstFreeSlotIndex()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemType == ItemType.None)
                return i;
        }
        return -1;
    }

    public void DropActiveItem(Transform dropPoint, Camera cam)
    {
        int slot = activeSlot;
        if (slots[slot].itemType == ItemType.None)
            return;

        GameObject dropObj = ItemDatabase.SpawnItem(slots[slot].itemType, dropPoint.position);
        if (dropObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb.AddForce(cam.transform.forward * 2f, ForceMode.Impulse);

        ClearSlot(slot);
    }

    #endregion

    #region Consume

    private void ConsumeActiveItem()
    {
        InventorySlot slot = slots[activeSlot];
        if (slot.consumableData == null)
            return;

        ConsumableData data = slot.consumableData;
        if (!data.isFood && !data.isDrink)
            return;

        PlayerStatsSystem stats = FindAnyObjectByType<PlayerStatsSystem>();
        if (stats == null)
            return;

        uiManager.ShowEKeyBind(true);

        if (consumeAction.action.WasPressedThisFrame())
        {
            if (data.isFood)
                stats.AddHunger(data.foodAmount);

            if (data.isDrink)
                stats.AddThirst(data.drinkAmount);

            if (data.sound != null && audioProps != null && !audioProps.isPlaying)
                audioProps.PlayOneShot(data.sound);

            ClearSlot(activeSlot);
        }
    }

    #endregion

    #region Helpers

    public bool HasFreeSlot()
    {
        foreach (var s in slotParents)
            if (s.childCount == 0)
                return true;

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

    #endregion
}