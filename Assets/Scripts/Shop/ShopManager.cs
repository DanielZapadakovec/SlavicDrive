using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class CartItem
    {
        public ItemType type;
        public int quantity = 1;
    }

    [Header("References")]
    public Transform cartContentParent;
    public GameObject cartItemPrefab;
    public Transform spawnPoint;
    public float spawnRadius = 1.5f;
    public GameObject ShopUI;

    bool isShowingUI;

    [Header("Money")]
    public PlayerStatsSystem playerStatsSystem;
    public Button buyButton;

    private List<CartItem> cartItems = new List<CartItem>();
    private List<GameObject> cartUIObjects = new List<GameObject>();

    void Start()
    {
        UpdateCartUI();
        buyButton.onClick.AddListener(BuyAll);
    }

    public void AddToCart(ItemType type)
    {
        CartItem existing = cartItems.Find(c => c.type == type);
        if (existing != null)
        {
            existing.quantity++;
        }
        else
        {
            cartItems.Add(new CartItem { type = type, quantity = 1 });
        }
        
        UpdateCartUI();
    }

    public void ShowShopUI()
    {
        if (!isShowingUI)
        {
            PlayerController.SwitchingCameraMovement();
            PlayerInteraction.canInteract = false;
            isShowingUI = true;  
            ShopUI.SetActive(true);
        }
        else if (isShowingUI)
        {
            PlayerController.SwitchingCameraMovement();
            isShowingUI = false;
            ShopUI.SetActive(false);
            PlayerInteraction.canInteract = true;
        }

    }
    void UpdateCartUI()
    {
        foreach (var go in cartUIObjects) Destroy(go);
        cartUIObjects.Clear();

        foreach (var item in cartItems)
        {
            GameObject go = Instantiate(cartItemPrefab, cartContentParent);

            Sprite icon = ItemDatabase.GetIcon(item.type);
            string itemName = item.type.ToString();
            int price = ItemDatabase.GetPrice(item.type);

            Image iconImage = go.transform.Find("Icon").GetComponent<Image>();
            Text nameText = go.transform.Find("Name").GetComponent<Text>();
            Text qtyText = go.transform.Find("Quantity").GetComponent<Text>();
            Text priceText = go.transform.Find("Price").GetComponent<Text>();

            if (iconImage) iconImage.sprite = icon;
            if (nameText) nameText.text = itemName;
            if (qtyText) qtyText.text = "x" + item.quantity;
            if (priceText) priceText.text = (item.quantity * price) + " €";

            cartUIObjects.Add(go);
        }
    }
    int GetTotalPrice()
    {
        int total = 0;
        foreach (var item in cartItems)
        {
            total += ItemDatabase.GetPrice(item.type) * item.quantity;
        }
        return total;
    }
    void BuyAll()
    {
        int totalPrice = GetTotalPrice();
        if (playerStatsSystem.currentMoney >= totalPrice)
        {
            playerStatsSystem.SubtractMoney(totalPrice);

            foreach (var item in cartItems)
            {
                GameObject prefab = ItemDatabase.GetPrefab(item.type);
                if (prefab != null)
                {
                    for (int i = 0; i < item.quantity; i++)
                    {
                        Vector3 spawnPos = spawnPoint.position + Random.insideUnitSphere * spawnRadius;
                        spawnPos.y = spawnPoint.position.y;
                        Instantiate(prefab, spawnPos, Quaternion.identity);
                    }
                }
            }

            cartItems.Clear();
            UpdateCartUI();
            ShowShopUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
