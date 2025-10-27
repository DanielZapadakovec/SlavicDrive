using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField]public Animator EKeyBindUI;
    [SerializeField]public Animator FKeyBindUI;

    public void ShowEKeyBind(bool canShow)
    {
        EKeyBindUI.SetBool("canShow", canShow);
    }
    public void ShowFKeyBind(bool canShow)
    {
        FKeyBindUI.SetBool("canShow", canShow);
    }
}
