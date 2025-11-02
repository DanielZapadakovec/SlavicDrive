using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ItemGrabber : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] LayerMask ObjectLayer;
    [SerializeField] float DetectDistance = 5f;
    [SerializeField] Transform dropPoint;
    [SerializeField] InventoryQuickBar inventory;

    [Header("UI")]
    public UIManager uiManager;
    public CrosshairNormal cross;

    [Header("Car Assembly")]
    public CarAssembly currentCar;
    [SerializeField] LayerMask CarPartLayer;

    private Camera cam;
    private RaycastHit hit;
    private ItemID outlinedItem;
    private bool canPickUp, canAssembly;

    void Start() => cam = GetComponent<Camera>();

    void Update()
    {
        HandleRaycast();
        HandleAssembly();
        UpdateUIHints();
    }

    void HandleRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, DetectDistance, ObjectLayer))
        {
            if (hit.transform.TryGetComponent(out ItemID item))
            {
                if (outlinedItem != item)
                {
                    if (outlinedItem != null) outlinedItem.DisableOutline();
                    outlinedItem = item;
                    outlinedItem.EnableOutline();
                }
                canPickUp = true;

                if (Input.GetKeyDown(KeyCode.E))
                    inventory.TryPickUpItem(outlinedItem, dropPoint, cam);
                return;
            }
        }

        if (outlinedItem != null) outlinedItem.DisableOutline();
        outlinedItem = null;
        canPickUp = false;

        if (Input.GetKeyDown(KeyCode.F))
            inventory.DropActiveItem(dropPoint, cam);
    }

    void HandleAssembly()
    {
        if (currentCar == null) return;

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

    void UpdateUIHints()
    {
        bool showE = canPickUp || canAssembly;
        uiManager.ShowEKeyBind(showE);

        int slotIndex = inventory.activeSlot;
        bool hasItemInHand = inventory.slots[slotIndex].itemType != ItemType.None;
        bool showF = !canPickUp && hasItemInHand;
        uiManager.ShowFKeyBind(showF);
    }
}