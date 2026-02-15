using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHoverButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public Vector3 hoverScale = Vector3.one * 1.05f;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null) { audioSource.PlayOneShot(hoverSound); }
        transform.localScale = hoverScale;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}


