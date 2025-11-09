using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public UIManager Instance;
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

    public bool IsPointerOverUI(GameObject parent)
    {
        if (EventSystem.current == null) return false;

        var pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var r in results)
        {
            if (r.gameObject == parent || r.gameObject.transform.IsChildOf(parent.transform))
                return true;
        }

        return false;
    }
}
