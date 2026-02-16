using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialoguePlayer dialoguePlayer;
    public DialogueAsset firstDialogue;
    public DialogueAsset loopDialogue;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger: Player");
        if ( other.CompareTag(("Player")) && !firstDialogue.hasPlayed)
        {
            dialoguePlayer.Play(firstDialogue);
            Debug.Log("Trigger: Player a dialog");
        }
        else if (other.CompareTag(("Player")) && firstDialogue.hasPlayed)
        {
            dialoguePlayer.Play(firstDialogue);
            Debug.Log("Trigger: Player a dialog2");
        }
    }
}
