using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{ [HideInInspector] public Transform parentAfterDrag;
    public Image iconSlot;
    public ItemType itemType;
    public void OnBeginDrag(PointerEventData eventData) 
    {
        Debug.Log("begindrag"); parentAfterDrag = transform.parent; transform.SetParent(transform.root); transform.SetAsLastSibling(); iconSlot.raycastTarget = false; 
    }
    public void OnDrag(PointerEventData eventData) 
    { 
        Debug.Log("drag");
        transform.position = Input.mousePosition; 
    }
    public void OnEndDrag(PointerEventData eventData) 
    { 
        Debug.Log("enddrag");
        transform.SetParent(parentAfterDrag); iconSlot.raycastTarget = true; } 
    }