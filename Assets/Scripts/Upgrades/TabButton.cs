using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public GameObject tabContent;
    public Vector3 selectedScale = Vector3.one * 1.1f;
    public Vector3 normalScale = Vector3.one;
    public Color selectColor;
    public Color deselectColor;
    public Image buttonImage;

    public void Select()
    {
        transform.localScale = selectedScale;
        tabContent.SetActive(true);
        buttonImage.color = selectColor;
    }

    public void Deselect()
    {
        transform.localScale = normalScale;
        tabContent.SetActive(false);
        buttonImage.color = deselectColor;
    }
}
