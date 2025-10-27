using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayer : MonoBehaviour
{
    public GameObject mapObject;
    public bool isOpen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isOpen)
        {
            OpenMap();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && isOpen)
        {
            CloseMap();
        }
    }

    public void OpenMap()
    {
        isOpen = true;
        mapObject.SetActive(true);
    }
    public void CloseMap()
    {
        isOpen = false;
        mapObject.SetActive(false);
    }
}
