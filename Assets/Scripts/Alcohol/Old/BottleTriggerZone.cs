using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleTriggerZone : MonoBehaviour
{
    public BottleCrateManager bottleManager;
    public AlcoholMachine alcoholMachine;

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "BottleCrate")
        {
            bottleManager = other.GetComponent<BottleCrateManager>();
            if (alcoholMachine.alcoholAmount > 0.15f && !bottleManager.isFull && alcoholMachine.isLevered)
            {
                bottleManager.AddBottle();
                alcoholMachine.alcoholAmount -= 0.15f;
            }
        }
        else { bottleManager = null; }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "BottleCrate")
        {
            bottleManager = null;
        }
    }
}
