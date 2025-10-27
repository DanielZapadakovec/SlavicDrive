using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    private Interactable currentInteractable;
    public Camera playerCamera;
    public CrosshairNormal cross;
    public Text interactableText;
    private bool canHold;
    public Text errorMessage;
    public bool isUsingItem;
    [SerializeField] public static bool canInteract = true;



    void Update()
    {
        CheckInteraction();
        if (Input.GetMouseButtonDown(0) && currentInteractable != null && !canHold)
        {
            currentInteractable.Interact();
            isUsingItem = true;
        }
        else if (canHold && currentInteractable != null) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                isUsingItem = true;
                currentInteractable.EnableHolding();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isUsingItem = false;
                currentInteractable.DisableHolding(); 
            }
        }

    }

    void CheckInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {

            if (hit.collider.CompareTag("Interactable") && canInteract)
            {
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
                    SetNewCurrentInteractable(newInteractable);
                }
            }
            else
            {
                isUsingItem = false;
                DisableCurrentInteractable();
            }

        }
        else
        {
            isUsingItem = false;
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        EnableInteractionText(currentInteractable.message);
        cross.InteractiveActive();
    }

    public void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            if(currentInteractable.canHold)
            {
                currentInteractable.DisableHolding();
            }
            cross.InteractiveBase();
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