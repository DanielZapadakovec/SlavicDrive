using UnityEngine;
using UnityEngine.UI;

public class StorageInventory : MonoBehaviour
{
    public Transform[] slots;
    public GameObject itemUIPrefab;

    public bool HasFreeSlot()
    {
        foreach (var s in slots)
            if (s.childCount == 0) return true;
        return false;
    }

    public void AddItem(ItemType type)
    {
        foreach (var s in slots)
        {
            if (s.childCount == 0)
            {
                var ui = Instantiate(itemUIPrefab, s);
                var clickable = ui.GetComponent<ClickableItem>();
                clickable.itemType = type;
                clickable.isInStorage = true;
                clickable.iconSlot.sprite = ItemDatabase.GetIcon(type);
                return;
            }
        }
    }

    public void RemoveItem(ItemType type)
    {
        foreach (var s in slots)
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
