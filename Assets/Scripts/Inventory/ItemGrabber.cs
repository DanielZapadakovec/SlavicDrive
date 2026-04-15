using UnityEngine;
using UnityEngine.UI;

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
    public CrosshairSystem cross;

    [Header("Car Assembly")]
    public CarAssembly currentCar;
    [SerializeField] LayerMask CarPartLayer;

    private Camera cam;
    private RaycastHit hit;
    private ItemID outlinedItem;
    public bool canPickUp, canAssembly;

    [Header("Assembly Hold Settings")]
    public float holdTimeRequired = 2f;
    private float holdTimer = 0f;
    private bool isHolding = false;
    public AudioSource propsAudioSource;
    public AudioClip assemblingAudioClip;

    void Start() => cam = GetComponent<Camera>();

    void Update()
    {
        HandleRaycast();
        HandleAssembly();
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
                    cross.SetCrosshair(CrosshairSystem.CrosshairType.Grabbable);
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
        cross.SetCrosshair(CrosshairSystem.CrosshairType.Base);

        if (Input.GetKeyDown(KeyCode.F))
            inventory.DropActiveItem(dropPoint, cam);
    }

    void HandleAssembly()
    {
        if (currentCar == null)
            return;

        int slotIndex = inventory.activeSlot;
        ItemType heldType = inventory.slots[slotIndex].itemType;

        // RESET, ak nemám item
        if (heldType == ItemType.None)
        {
            ResetAssembly();
            currentCar.HideAllPreviews();
           // crosshair.SetMountable(false);
            return;
        }

        currentCar.ShowSlotPreview(heldType, true);


        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out RaycastHit hit,DetectDistance,CarPartLayer))
        {
            canAssembly = true;
            cross.SetCrosshair(CrosshairSystem.CrosshairType.Special);
            //crosshair.SetMountable(true);

            if (Input.GetKey(KeyCode.E))
            {
                isHolding = true;
                holdTimer += Time.deltaTime;

                uiManager.progressImage.gameObject.SetActive(true);
                uiManager.progressImage.fillAmount = holdTimer / holdTimeRequired;
                if (!propsAudioSource.isPlaying)
                {
                    propsAudioSource.PlayOneShot(assemblingAudioClip);
                }

                if (holdTimer >= holdTimeRequired)
                {
                    CompleteAssembly(heldType, hit, slotIndex);
                }
            }
            else
            {
                ResetHold();
            }
        }
        else
        {
            ResetAssembly();
            cross.SetCrosshair(CrosshairSystem.CrosshairType.Base);
        }
    }

    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        if (uiManager.progressImage != null)
            uiManager.progressImage.fillAmount = 0f;
        propsAudioSource.Stop();
    }

    void ResetAssembly()
    {
        ResetHold();
        canAssembly = false;

        if (uiManager.progressImage != null)
            uiManager.progressImage.gameObject.SetActive(false);

       // crosshair.SetMountable(false);
    }
    void CompleteAssembly(ItemType heldType, RaycastHit hit, int slotIndex)
    {
        if (currentCar.TryInstallPart(heldType, hit))
        {
            // 🎞️ FAKE ANIMATION HOOK
          //  currentCar.AnimateMount(heldType, hit.point);

            inventory.ClearSlot(slotIndex);
        }

        ResetAssembly();
    }


}