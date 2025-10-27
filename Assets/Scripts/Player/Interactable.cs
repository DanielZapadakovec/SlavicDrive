using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    Outline outline;
    public string message;

    public UnityEvent onInteraction;

    public AudioClip clickSound;
    private AudioSource audioSource;

    [Header("Holding")]
    [SerializeField] public bool canHold;
    public bool isHolding;
    void Start()
    {
        outline = GetComponent<Outline>();
        audioSource = GetComponent<AudioSource>(); 
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        DisableOutline();
    }
    private void Update()
    {
        if (isHolding && canHold)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        onInteraction.Invoke();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }
    public void EnableHolding()
    {
        isHolding = true;

    }
    public void DisableHolding() 
    {
        isHolding = false;
    }
}
