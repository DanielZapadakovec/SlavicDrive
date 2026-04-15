using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    private Interactable currentInteractable;
    public Camera playerCamera;
    public CrosshairSystem cross;
    public Text interactableText;
    private bool canHold;
    public Text errorMessage;
    public bool isUsingItem;
    public bool isInteracting;
    [SerializeField] public static bool canInteract = true;
    public UIManager uiManager;
    public InventoryQuickBar inventoryQuickBar;


    void Update()
    {
        CheckInteraction();
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && !canHold)
        {
            currentInteractable.Interact();
            isUsingItem = true;
        }
        else if (canHold && currentInteractable != null) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isUsingItem = true;
                currentInteractable.EnableHolding();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                isUsingItem = false;
                currentInteractable.DisableHolding(); 
            }
        }

    }
    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetActivePlayerInteraction(this);
    }
    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetActivePlayerInteraction(null);
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {

            if (hit.collider.CompareTag("Interactable") && canInteract)
            {
                inventoryQuickBar.canConsume = false;
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();
                if (newInteractable.canHold)
                {
                    canHold = true;
                    
                }
                else { canHold = false; isUsingItem = false ; }
                if (newInteractable == null)
                {
                    return;
                }

                if (currentInteractable && newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }

                if (newInteractable.enabled)
                {
                    isInteracting = true;
                    SetNewCurrentInteractable(newInteractable);
                }
            }
            else
            {
                isInteracting = false;
                inventoryQuickBar.canConsume = true;
                isUsingItem = false;
                DisableCurrentInteractable();
            }

        }
        else
        {
            isInteracting = false;
            inventoryQuickBar.canConsume = true;
            isUsingItem = false;
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        EnableInteractionText(currentInteractable.message);
        cross.SetCrosshair(CrosshairSystem.CrosshairType.Interactable);
    }

    public void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            if(currentInteractable.canHold)
            {
                currentInteractable.DisableHolding();
            }
            cross.SetCrosshair(CrosshairSystem.CrosshairType.Base);
            errorMessage.text = null;
            currentInteractable.DisableOutline();
            DisableInteractionText();
            currentInteractable = null;
        }
    }

    public void EnableInteractionText(string text)
    {
        interactableText.text = text;
        interactableText.gameObject.SetActive(true);
    }
    public void DisableInteractionText()
    {
        interactableText.gameObject.SetActive(false);
    }
}