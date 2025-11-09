using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public Image iconSlot;
    public ItemType itemType;
    public bool isInStorage;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.MoveItem(this, isInStorage);
            }
        }
    }
}