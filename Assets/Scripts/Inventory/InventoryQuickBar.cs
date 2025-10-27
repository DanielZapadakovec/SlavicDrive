using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryQuickBar : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType itemType = ItemType.None;
        public GameObject itemPrefabInstance;
        public ConsumableData consumableData;
    }

    public InventorySlot[] slots = new InventorySlot[4];
    public Transform[] slotParents;
    public int activeSlot = 0;

    [Header("Prefabs")]
    public GameObject itemUIPrefab;

    [Header("Player Hand")]
    public Transform handTransform;
    public GameObject currentHeldItem;

    [Header("UI Highlight")]
    public Color normalColor = new Color(1, 1, 1, 0.6f);
    public Color activeColor = new Color(1, 1, 1, 1f);

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
            ClearSlot(i);

        UpdateUIHighlight();
    }

    void Update()
    {
        HandleSlotSwitch();
        CheckIfItemStillExists();
    }

    void HandleSlotSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveSlot(3);
    }

    public void SetActiveSlot(int index)
    {
        activeSlot = index;
        UpdateUIHighlight();
        RefreshSlotData(index);
        UpdateHeldItem();
    }
    private void UpdateUIHighlight()
    {
        for (int i = 0; i < slotParents.Length; i++)
        {
            if (slotParents[i].childCount > 0)
            {
                DraggableItem draggable = slotParents[i].GetChild(0).GetComponent<DraggableItem>();
                if (draggable != null && draggable.iconSlot != null)
                {
                    draggable.iconSlot.color = (i == activeSlot) ? activeColor : normalColor;
                }
            }
        }
    }

    public void SetSlot(int index, ItemType type, Sprite icon, ConsumableData consumable)
    {
        ClearSlot(index);

        slots[index].itemType = type;
        slots[index].consumableData = consumable;

        GameObject itemUI = Instantiate(itemUIPrefab, slotParents[index]);
        DraggableItem itemUIScript = itemUI.GetComponent<DraggableItem>();
        itemUIScript.itemType = type;
        itemUIScript.iconSlot.sprite = icon;

        slots[index].itemPrefabInstance = itemUI;

        if (index == activeSlot)
        {
            RefreshSlotData(index);
            UpdateHeldItem();
        }

        UpdateUIHighlight();
    }

    public void ClearSlot(int index)
    {
        slots[index].itemType = ItemType.None;
        slots[index].consumableData = null;

        if (slots[index].itemPrefabInstance != null)
            Destroy(slots[index].itemPrefabInstance);

        slots[index].itemPrefabInstance = null;

        if (index == activeSlot)
            UpdateHeldItem();
    }

    private void UpdateHeldItem()
    {
        if (currentHeldItem != null)
            Destroy(currentHeldItem);

        var slot = slots[activeSlot];
        if (slot.itemType != ItemType.None)
        {
            GameObject prefab = ItemDatabase.GetPrefab(slot.itemType);

            currentHeldItem = Instantiate(prefab, handTransform);
            currentHeldItem.transform.localPosition = Vector3.zero;
            currentHeldItem.transform.localRotation = Quaternion.identity;

            Rigidbody rb = currentHeldItem.GetComponent<Rigidbody>();
            Collider col = currentHeldItem.GetComponent<Collider>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    public void ConsumeActiveSlot()
    {
        int index = activeSlot;
        var data = slots[index].consumableData;
        if (data != null)
        {
            var playerStats = FindAnyObjectByType<PlayerStatsSystem>();
            if (data.isFood) playerStats.AddHunger(data.foodAmount);
            if (data.isDrink) playerStats.AddThirst(data.drinkAmount);

            if (data.sound != null)
                AudioSource.PlayClipAtPoint(data.sound, Camera.main.transform.position);

            ClearSlot(index);
        }
    }

    private void CheckIfItemStillExists()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slotParents[i].childCount == 0)
            {
                slots[i].itemType = ItemType.None;
                slots[i].consumableData = null;
                slots[i].itemPrefabInstance = null;
            }
            else
            {
                Transform child = slotParents[i].GetChild(0);
                DraggableItem draggable = child.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    slots[i].itemType = draggable.itemType;

                    if (slots[i].consumableData == null)
                        slots[i].consumableData = ItemDatabase.GetConsumableData(draggable.itemType);

                    slots[i].itemPrefabInstance = child.gameObject;
                }
            }
        }
    }

    private void RefreshSlotData(int index)
    {
        var type = slots[index].itemType;
        if (type == ItemType.None) return;

        slots[index].consumableData = ItemDatabase.GetConsumableData(type);
        Sprite icon = ItemDatabase.GetIcon(type);

        if (slots[index].itemPrefabInstance != null)
        {
            var draggable = slots[index].itemPrefabInstance.GetComponent<DraggableItem>();
            if (draggable != null && draggable.iconSlot != null)
                draggable.iconSlot.sprite = icon;
        }
    }
}
