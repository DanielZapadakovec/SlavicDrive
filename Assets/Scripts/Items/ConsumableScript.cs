using UnityEngine;

[System.Serializable]
public class ConsumableData
{
    public bool isFood;
    public float foodAmount;
    public bool isDrink;
    public float drinkAmount;
    public AudioClip sound;   // zvuk ktorý sa prehrá pri konzumácii
}

public class ConsumableScript : MonoBehaviour
{
    [Header("Consumable Setup")]
    public ConsumableData data;   // tu sa definuje èo je to za item
    public AudioSource audioSource;

    private PlayerStatsSystem playerStatsSystem;
    private Rigidbody objectRigidbody;
    private Outline objectOutline;
    private Interactable interactable;
    private ItemID grabbableItemID;

    void Start()
    {
        playerStatsSystem = FindAnyObjectByType<PlayerStatsSystem>();
        objectRigidbody = GetComponent<Rigidbody>();
        objectOutline = GetComponent<Outline>();
        interactable = GetComponent<Interactable>();
        grabbableItemID = GetComponent<ItemID>();
    }

    public void Consume()
    {
        if (data == null) return;

        if (data.isFood)
        {
            playerStatsSystem.AddHunger(data.foodAmount);
            if (data.sound != null) audioSource.PlayOneShot(data.sound);
        }
        else if (data.isDrink)
        {
            playerStatsSystem.AddThirst(data.drinkAmount);
            if (data.sound != null) audioSource.PlayOneShot(data.sound);
        }
    }

    // volá sa pri kúpe v shope
    public void BuyObject(float price)
    {
        if (playerStatsSystem.currentMoney >= price)
        {
            objectRigidbody.useGravity = true;
            interactable.enabled = false;
            grabbableItemID.enabled = true;
            gameObject.layer = 7;

            objectOutline.OutlineColor = Color.white;
            playerStatsSystem.SubtractMoney(price);
        }
    }

    // Getter pre Inventory – vytiahne všetky dáta o iteme
    public ConsumableData GetData()
    {
        return data;
    }
}
