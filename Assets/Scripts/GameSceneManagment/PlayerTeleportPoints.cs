using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportPoints : MonoBehaviour
{
    [Header("Teleport Targets")]
    public Transform Home;
    public Transform City;
    public Transform Scrapyard;

    [Header("Player Transform")]
    [Tooltip("Transform objektu hr·Ëa, ktor˝ sa bude teleportovaù.")]
    public Transform player;


    public void TeleportToHome()
    {
            player.SetPositionAndRotation(Home.position, Home.rotation);

    }
   public  void TeleportToCity()
    {
        player.SetPositionAndRotation(City.position, City.rotation);

    }
    public void TeleportToScrapyard()
    {
        player.SetPositionAndRotation(Scrapyard.position, Scrapyard.rotation);

    }
}
