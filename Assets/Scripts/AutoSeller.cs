using System.Collections.Generic;
using UnityEngine;

public class AutoSeller : MonoBehaviour
{
    [System.Serializable]
    public class ItemSellData
    {
        public ItemType itemType;
        public int price;
    }
    [Header("References")]
    public StorageObject storage;
    public DayNightCycle dayNight;
    public PlayerStatsSystem playerStats;

    [Header("Sell Settings")]
    public bool isActive = true;
    public float sellIntervalHours = 1f;
    private int hourCounter = 0;
    public int sellEveryXHours = 1;

    public List<ItemSellData> sellableItems = new List<ItemSellData>();

    private float hourTimer = 0f;

    private void Start()
    {
        if (dayNight == null)
            dayNight = FindObjectOfType<DayNightCycle>();

        if (dayNight != null)
            dayNight.OnHourChanged += OnHourPassed;
    }
    void OnHourPassed(int currentHour)
    {
        if (!isActive || storage == null)
            return;

        hourCounter++;

        if (hourCounter >= sellEveryXHours)
        {
            hourCounter = 0;
            SellOneItem();
        }
    }

    private void OnDestroy()
    {
        if (dayNight != null)
            dayNight.OnHourChanged -= OnHourPassed;
    }
    void Update()
    {
        if (!isActive || storage == null) return;

        hourTimer += Time.deltaTime;

        if (hourTimer >= GetSecondsPerGameHour() * sellIntervalHours)
        {
            hourTimer = 0f;
            SellOneItem();
        }
    }

    float GetSecondsPerGameHour()
    {
        DayNightCycle cycle = FindObjectOfType<DayNightCycle>();
        if (cycle != null)
            return 3600f / cycle.timeMultiplier;

        return 60f; // fallback
    }

    void SellOneItem()
    {
        foreach (var data in sellableItems)
        {
            if (storage.CountItem(data.itemType) > 0)
            {
                storage.RemoveItems(data.itemType, 1);

                if (playerStats != null)
                    playerStats.AddMoney(data.price);

                Debug.Log($"Sold 1x {data.itemType} for {data.price}");
                return; // len 1 item za interval
            }
        }
    }
}
