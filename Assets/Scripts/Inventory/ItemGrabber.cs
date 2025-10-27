using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class ItemGrabber : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] LayerMask ObjectLayer = 1;
    [SerializeField] float DetectDistance = 5f;
    [SerializeField] Transform dropPoint; // kam sa položí item pri výmene
    [SerializeField] InventoryQuickBar inventory;

    [Header("UI flag")]
    public bool canPickUp;
    public UIManager uiManager;
    public CrosshairNormal cross;

    private Camera cam;
    private RaycastHit hit;
    private ItemID outlinedItem;

    [Header("Car Assembly")]
    public CarAssembly currentCar; // nastaví CarAssembly keď si v jeho triggeri
    [SerializeField] LayerMask CarPartLayer;
    bool canAssembly;

    [Header("Consumables")]
    bool canConsume;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleConsume();
        bool hitSomething = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, DetectDistance, ObjectLayer);
        if (hitSomething && hit.transform.TryGetComponent(out ItemID grab))
        {
            HandleItemPickup(grab);
        }
        else
        {
            HandleNoPickupHit();
        }

        if (currentCar != null)
        {
            HandleCarAssembly();
        }

        UpdateUIHints();
    }

    void HandleItemPickup(ItemID grab)
    {
        if (outlinedItem != grab)
        {
            if (outlinedItem != null) outlinedItem.DisableOutline();
            outlinedItem = grab;
            outlinedItem.EnableOutline();
        }
        canPickUp = true;
        uiManager.ShowFKeyBind(false);

        if (Input.GetKeyDown(KeyCode.E) && !canAssembly)
        {
            TryPickUp(outlinedItem);
        }
    }

    void HandleNoPickupHit()
    {
        if (outlinedItem != null) outlinedItem.DisableOutline();
        outlinedItem = null;
        canPickUp = false;
        uiManager.ShowFKeyBind(true);

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryDropFromInventory();
        }
    }

    void HandleCarAssembly()
    {
        int slotIndex = inventory.activeSlot;
        ItemType heldType = inventory.slots[slotIndex].itemType;

        if (heldType == ItemType.None)
        {
            currentCar.HideAllPreviews();
            return;
        }

        currentCar.ShowSlotPreview(heldType, true);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, DetectDistance, CarPartLayer))
        {
            canAssembly = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentCar.TryInstallPart(heldType, hit))
                {
                    inventory.ClearSlot(slotIndex);
                    canAssembly = false;
                }
            }
        }
        else
        {
            canAssembly = false;
        }
    }

    public void HandleConsume()
    {
        int slotIndex = inventory.activeSlot;
        var slot = inventory.slots[slotIndex];

        canConsume = false;

        if (slot.itemType == ItemType.None) return;
        if (slot.consumableData == null) return;

        canConsume = true;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ConsumableData data = slot.consumableData;
            PlayerStatsSystem stats = FindAnyObjectByType<PlayerStatsSystem>();

            if (data != null && stats != null)
            {
                if (data.isFood)
                {
                    stats.AddHunger(data.foodAmount);
                }
                else if (data.isDrink)
                {
                    stats.AddThirst(data.drinkAmount);
                }
            }

            inventory.ClearSlot(slotIndex);
            canConsume = false;
        }
    }

    void TryPickUp(ItemID item)
    {
        int slot = inventory.activeSlot;

        if (inventory.slots[slot].itemType != ItemType.None)
        {
            GameObject oldObj = ItemDatabase.SpawnItem(inventory.slots[slot].itemType, dropPoint.position);
            oldObj.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 2f, ForceMode.Impulse);
        }

        ItemType type = item.itemType;
        Sprite icon = ItemDatabase.GetIcon(type);

        ConsumableData consumable = null;
        if (item.TryGetComponent<ConsumableScript>(out var consumableScript))
        {
            consumable = consumableScript.GetData();
        }

        inventory.SetSlot(slot, type, icon, consumable);

        Destroy(item.gameObject);
    }

    void TryDropFromInventory()
    {
        int slot = inventory.activeSlot;

        if (inventory.slots[slot].itemType != ItemType.None)
        {
            GameObject dropObj = ItemDatabase.SpawnItem(inventory.slots[slot].itemType, dropPoint.position);
            if (dropObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(cam.transform.forward * 2f, ForceMode.Impulse);
            }

            inventory.ClearSlot(slot);
        }
    }

    void UpdateUIHints()
    {
        bool showE = canPickUp || canAssembly || canConsume;
        uiManager.ShowEKeyBind(showE);

        int slotIndex = inventory.activeSlot;
        bool hasItemInHand = inventory.slots[slotIndex].itemType != ItemType.None;

        bool showF = !canPickUp && hasItemInHand;
        uiManager.ShowFKeyBind(showF);
    }
}
