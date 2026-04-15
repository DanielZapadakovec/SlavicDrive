using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportHome : MonoBehaviour
{
    public GameObject Player;
    public Transform houseTransform;

    public void Teleport()
    {
        Player.transform.position = houseTransform.position;
        Player.transform.rotation = houseTransform.rotation;
    }
}
