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


    // Getter pre Inventory – vytiahne všetky dáta o iteme
    public ConsumableData GetData()
    {
        return data;
    }
}
