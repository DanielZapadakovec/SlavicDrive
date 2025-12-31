using System.Collections.Generic;
using UnityEngine;

public class StorageItemVisualizer : MonoBehaviour
{
    [Header("Reference to Storage")]
    public StorageObject storage;

    [Header("Transforms where items will be shown")]
    public List<Transform> spawnPoints = new List<Transform>();

    private List<GameObject> spawnedVisualItems = new List<GameObject>();

    private void Start()
    {
        RefreshVisualization();
    }

    private void Update()
    {
        // Kontrola, èi sa nieèo nezmenilo
        if (spawnedVisualItems.Count != storage.storedItems.Count)
        {
            RefreshVisualization();
        }
        else
        {
            // Alebo kontrola zmien v poradí
            for (int i = 0; i < storage.storedItems.Count; i++)
            {
                if (spawnedVisualItems[i] == null ||
                    spawnedVisualItems[i].name != storage.storedItems[i].ToString())
                {
                    RefreshVisualization();
                    break;
                }
            }
        }
    }

    public void RefreshVisualization()
    {
        // Vymažeme staré
        foreach (var obj in spawnedVisualItems)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedVisualItems.Clear();

        // Pre každý item v storage vytvoríme jeho vizualizáciu
        for (int i = 0; i < storage.storedItems.Count; i++)
        {
            if (i >= spawnPoints.Count)
            {
                Debug.LogWarning("Nedostatok spawn pointov pre vizualizáciu!");
                break;
            }

            ItemType type = storage.storedItems[i];

            GameObject prefab = ItemDatabase.GetPrefab(type);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab pre item " + type + " nebol nájdený!");
                continue;
            }

            Transform targetPoint = spawnPoints[i];

            GameObject spawned = Instantiate(prefab, targetPoint.position, targetPoint.rotation, targetPoint);

            spawned.name = type.ToString();

            // Layer na Default
            spawned.layer = 0;

            // Vypneme fyziku
            /*Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if (rb != null)
            {
              //  rb.useGravity = false;
                //rb.isKinematic = true;
            }*/

            // Ak prefab má Collider – odporúèanie: nechajme ho, ale vypnutý
           /* Collider col = spawned.GetComponent<Collider>();
            if (col != null)
              //  col.enabled = false;*/

            spawnedVisualItems.Add(spawned);
        }
    }
}
