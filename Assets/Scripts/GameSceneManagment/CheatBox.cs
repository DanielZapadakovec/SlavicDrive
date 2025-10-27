using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatBox : MonoBehaviour
{

    public bool isOpen;
    public GameObject cheatBox;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.F)) && !isOpen)
        {
            OpenCheatBox();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.F))&& isOpen)
        {
            CloseCheatBox();
        }
    }

    public void OpenCheatBox()
    {
        cheatBox.SetActive(true);
        isOpen= true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseCheatBox()
    {
        cheatBox.SetActive(false);
        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
