using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOffTrigger : MonoBehaviour
{
    public Collider objectCollider;
    public CarInteractables carInteractables;
    public bool colliderActive;


    public void OnTriggerStay(Collider other)
    {
        if (other.tag != null && carInteractables.isOpen)
        {
            objectCollider.enabled = false;
        }
        else
        {
            objectCollider.enabled = true;
        }
    }

    public void Update()
    {
        if (!colliderActive)
        {
            objectCollider.enabled = false;
        }
    }
}
