using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlcoholSellingPoint : MonoBehaviour
{
    [Header("Bottle Selling Settings")]
    public float bottleSellPrice = 25f;

    private BottleCrateManager detectedCrate;
    private bool isCrateInTrigger = false;
    public AudioSource sellAudioSource;
    public AudioClip sellAudioClip;

    [Header("References")]
    public PlayerStatsSystem moneySystem;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BottleCrate"))
        {
            BottleCrateManager crate = other.GetComponent<BottleCrateManager>();
            detectedCrate = crate;
            isCrateInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BottleCrateManager>() == detectedCrate)
        {
            detectedCrate = null;
            isCrateInTrigger = false;
        }
    }

    public void SellBottlesFromCrate()
    {
        if (isCrateInTrigger && detectedCrate != null)
        {
            int bottleCount = detectedCrate.transform.childCount;

            if (bottleCount <= 0)
            {
                return;
            }

            float totalEarned = bottleCount * bottleSellPrice;
            moneySystem.AddMoney(totalEarned);
            Debug.Log($"Predaných {bottleCount} fliaš za {totalEarned:0.00} €");
            sellAudioSource.PlayOneShot(sellAudioClip);
            detectedCrate.ClearCrate();
        }
    }
}
