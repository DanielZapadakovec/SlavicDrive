using System.Collections.Generic;
using UnityEngine;

public class FruitTreeController : MonoBehaviour
{
    [Header("Fruit Settings")]
    public GameObject fruitPrefab;                 // Prefab ovocia
    public int maxFruitCount = 5;                  // Koæko ovocia sa mÙûe naraz zrodiù
    public float spawnInterval = 5f;               // »as medzi spawnmi

    [Header("Spawn Points")]
    public Transform[] fruitSpawnPoints;           // Miesta, kde sa mÙûe ovocie objaviù

    private readonly List<GameObject> spawnedFruits = new List<GameObject>();
    private float timer;

    void Update()
    {
        spawnedFruits.RemoveAll(fruit => fruit == null);

        // Spawn kontrola
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnFruit();
        }
    }

    private void TrySpawnFruit()
    {
        if (spawnedFruits.Count >= maxFruitCount)
            return;

        Transform spawnPoint = fruitSpawnPoints[Random.Range(0, fruitSpawnPoints.Length)];

        GameObject newFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedFruits.Add(newFruit);
    }

    public void RemoveFruit(GameObject fruit)
    {
        if (spawnedFruits.Contains(fruit))
            spawnedFruits.Remove(fruit);
    }
}
