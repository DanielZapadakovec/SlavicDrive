using System.Collections.Generic;
using UnityEngine;

public class InventorySystemBase : MonoBehaviour
{
    [System.Serializable]
    public class StoredItem
    {
        public ItemType type;
        public int quantity;
        public ConsumableData consumableData;
        public IngredientData ingredientData;
    }

    [Header("Inventory")]
    public List<StoredItem> items = new();
    public int maxSlots = 20;
    public System.Action onChanged;

    public virtual void Add(ItemType type, int amount = 1)
    {
        // Stackovanie ak existuje
        foreach (var item in items)
        {
            if (item.type == type)
            {
                item.quantity += amount;
                onChanged?.Invoke();
                return;
            }
        }

        // Nový slot
        if (items.Count < maxSlots)
        {
            items.Add(new StoredItem
            {
                type = type,
                quantity = amount,
                consumableData = ItemDatabase.GetConsumableData(type),
                ingredientData = ItemDatabase.GetIngredientData(type)
            });
            onChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning("Inventár je plný!");
        }
    }

    public virtual bool Remove(ItemType type, int amount = 1)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].type == type)
            {
                items[i].quantity -= amount;
                if (items[i].quantity <= 0)
                    items.RemoveAt(i);
                onChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public StoredItem GetItem(ItemType type) => items.Find(i => i.type == type);
    public List<StoredItem> GetAll() => new List<StoredItem>(items);

    public void Clear()
    {
        items.Clear();
        onChanged?.Invoke();
    }
}
