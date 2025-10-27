using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageInteractables : MonoBehaviour
{
    [Header("GarageDoors")]
    public Animator doorAnimator;
    public AudioClip doorOpening;   
    public AudioClip doorClosing;
    bool isDoorOpen = true;
    public AudioSource doorAudioSource;

    #region GarageDoorInteract
    public void DoorInteract()
    {
        if (!isDoorOpen)
        {
            doorAnimator.SetBool("isOpen", true);
            isDoorOpen = true;
            StartCoroutine(PlayAudioBetter(3, doorOpening));
        }
        else
        {
            doorAnimator.SetBool("isOpen", false); 
            isDoorOpen = false;
            StartCoroutine(PlayAudioBetter(3, doorClosing));
        }
    }
    #endregion

    public IEnumerator PlayAudioBetter(float seconds, AudioClip audioclip)
    {
        doorAudioSource.clip = audioclip;
        doorAudioSource.Play();
        yield return new WaitForSeconds(seconds);
        doorAudioSource.Stop();
    }

}
