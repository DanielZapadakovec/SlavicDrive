using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicipalScript : MonoBehaviour
{
    public GameObject officeUI;
    public AudioSource ambienceAudio;

    private bool isOpen;

    public void ToggleOffice()
    {
        isOpen = !isOpen;
        officeUI.SetActive(isOpen);

        if (isOpen)
        {
            ambienceAudio?.Play();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ambienceAudio?.Stop();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
