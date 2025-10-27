using System.Collections.Generic;
using UnityEngine;

public class BottleCrateManager : MonoBehaviour
{
    [Header("Bottle Crate Settings")]
    public GameObject bottlePrefab;
    public int columns = 4;
    public int rows = 3;
    public int layers = 1;
    public bool isFull;

    public float spacingX = 0.2f;
    public float spacingY = 0.3f;
    public float spacingZ = 0.2f;


    [Header("Optional Spawn Origin")]
    public Transform spawnOrigin; 

    private List<GameObject> bottles = new List<GameObject>();
    [Header("Sounds")]
    public AudioSource crateAudioSource;

    public void AddBottle()
    {
        if (bottlePrefab == null) return;

        int index = bottles.Count;

        int x = index % columns;
        int z = (index / columns) % rows;
        int y = index / (columns * rows);

        if (y >= layers)
        {
            Debug.Log("Crate is full!");
            isFull = true;
            return;
        }
        else
        {
            isFull = false;
        }

        float totalWidth = (columns - 1) * spacingX;
        float totalHeight = (layers - 1) * spacingY;
        float totalDepth = (rows - 1) * spacingZ;

        Vector3 centerOffset = new Vector3(totalWidth, totalHeight, totalDepth) * -0.5f;

        Vector3 localOffset = new Vector3(
            x * spacingX,
            y * spacingY,
            z * spacingZ
        ) + centerOffset;

        Vector3 finalPosition;
        Quaternion finalRotation;

        if (spawnOrigin != null)
        {
            finalPosition = spawnOrigin.TransformPoint(localOffset); 
            finalRotation = spawnOrigin.rotation;
        }
        else
        {
            finalPosition = transform.TransformPoint(localOffset);
            finalRotation = transform.rotation;
        }
        crateAudioSource.Play();
        GameObject newBottle = Instantiate(bottlePrefab, finalPosition, finalRotation, transform);
        bottles.Add(newBottle);
    }

    public void ClearCrate()
    {
        foreach (GameObject bottle in bottles)
        {
            Destroy(bottle);
        }
        bottles.Clear();
    }
}
