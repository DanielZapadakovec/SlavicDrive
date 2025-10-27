using UnityEngine;

public class DoorOpen : MonoBehaviour
{


    [Header("Animator")]
    public Animator animator;
    public string animatorbool = "isOpen";
    public bool isOpen;
    [Header("Audio")]
    public AudioSource doorAudio;
    public AudioClip doorClipOpen;
    public AudioClip doorClipClose;

    public void DoorInteract()
    {
        if(!isOpen)
        {
            animator.SetBool(animatorbool, true);
            isOpen = true;
            doorAudio.clip = doorClipOpen;
            doorAudio.Play();
        }
        else
        {
            animator.SetBool(animatorbool, false);
            isOpen = false;
            doorAudio.clip = doorClipClose;
            doorAudio.Play();
        }
    }
}