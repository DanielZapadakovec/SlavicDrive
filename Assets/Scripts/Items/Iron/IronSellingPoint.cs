using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSellingPoint : MonoBehaviour
{
    [Header("Iron Selling Settings")]
    public float ironSellPrice = 50f;

    private bool isIronInTrigger = false;
    public AudioSource sellAudioSource;
    public AudioClip sellAudioClip;
    Collider iron;

    [Header("References")]
    public PlayerStatsSystem moneySystem;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Iron"))
        {
            iron = other;
            isIronInTrigger = true;
        }
    }


    public void SellIron()
    {
        if (isIronInTrigger)
        {
            moneySystem.AddMoney(ironSellPrice);
            sellAudioSource.PlayOneShot(sellAudioClip);
            Destroy(iron.gameObject);
            isIronInTrigger=false;
        }

    }
}
