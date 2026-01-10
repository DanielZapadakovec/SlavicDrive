using Unity.VisualScripting;
using UnityEngine;

public class ShopItemInteractable : MonoBehaviour
{
    public ItemType itemType;
    private ShopManager shopManager;

    public void Start()
    {
        shopManager = FindAnyObjectByType<ShopManager>();
    }
    public void AddThisToCart()
    {

        if (shopManager != null)
        {
            shopManager.AddToCart(itemType);
            this.gameObject.SetActive(false);
        }
    }
}
