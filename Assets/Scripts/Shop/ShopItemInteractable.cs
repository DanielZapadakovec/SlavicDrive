using Unity.VisualScripting;
using UnityEngine;

public class ShopItemInteractable : MonoBehaviour
{
    public ItemType itemType;              // vyber v inšpektore ktorý item to je
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
