#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemEntry
    {
        public ItemType type;
        public GameObject prefab;
        public Sprite icon;
        public int price = 10;
        public ConsumableData consumableData;
        public IngredientData ingredientData;
    }

    [Header("Databáza Itemov")]
    public List<ItemEntry> items = new List<ItemEntry>();

    private static ItemDatabase instance;
    public static ItemDatabase Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<ItemDatabase>("ItemDatabase");
            return instance;
        }
    }

    private void OnValidate()
    {
        SyncWithEnum();
    }

    private void SyncWithEnum()
    {
        Array enumValues = Enum.GetValues(typeof(ItemType));
        foreach (ItemType type in enumValues)
        {
            if (type == ItemType.None) continue;

            var entry = items.Find(e => e.type == type);
            if (entry == null)
            {
                entry = new ItemEntry { type = type, price = 10 };
                items.Add(entry);
            }

            // Automaticky priradí prefab z Resources/Prefabs
            if (entry.prefab == null)
            {
                GameObject prefab = Resources.Load<GameObject>($"Prefabs/{type}");
                if (prefab != null)
                {
                    entry.prefab = prefab;
#if UNITY_EDITOR
                    Debug.Log($"✅ Prefab pre {type} nastavený automaticky.");
#endif
                }
            }

            // Automaticky priradí ikonku z Resources/Icons
            if (entry.icon == null)
            {
                Sprite icon = Resources.Load<Sprite>($"Icons/{type}");
                if (icon != null)
                {
                    entry.icon = icon;
#if UNITY_EDITOR
                    Debug.Log($"✅ Icon pre {type} nastavená automaticky.");
#endif
                }
            }
        }

        // Odstráni entry, ktoré už nie sú v enum
        items.RemoveAll(e => !Enum.IsDefined(typeof(ItemType), e.type));
    }

    public static GameObject GetPrefab(ItemType type)
    {
        var entry = Instance.items.Find(i => i.type == type);
        return entry != null ? entry.prefab : null;
    }

    public static Sprite GetIcon(ItemType type)
    {
        var entry = Instance.items.Find(i => i.type == type);
        return entry != null ? entry.icon : null;
    }

    public static int GetPrice(ItemType type)
    {
        var entry = Instance.items.Find(i => i.type == type);
        return entry != null ? entry.price : 0;
    }

    public static ConsumableData GetConsumableData(ItemType type)
    {
        var entry = Instance.items.Find(i => i.type == type);
        return entry != null ? entry.consumableData : null;
    }

    public static IngredientData GetIngredientData(ItemType type)
    {
        var entry = Instance.items.Find(i => i.type == type);
        return entry != null ? entry.ingredientData : null;
    }

    public static GameObject SpawnItem(ItemType type, Vector3 position, Quaternion rotation = default)
    {
        var prefab = GetPrefab(type);
        if (prefab != null)
            return GameObject.Instantiate(prefab, position, rotation);
        Debug.LogWarning($"❌ Prefab pre {type} neexistuje!");
        return null;
    }
}
