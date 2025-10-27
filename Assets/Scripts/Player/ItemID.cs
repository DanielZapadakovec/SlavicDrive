using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemID : MonoBehaviour
{

    Outline outline;
    [Header("Item:")]
    public ItemType itemType = ItemType.None;
    private void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline();
    }
    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }
}
