using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHoverButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip hoverSound;

    [Header("Scale Settings")]
    public Vector3 hoverScale = Vector3.one * 1.05f;
    public float scaleLerpSpeed = 10f;

    [Header("Text Settings")]
    public Text buttonText;
    public Color textColorNormal = Color.white;
    public Color textColorHighlighted = Color.yellow;
    public float colorLerpSpeed = 10f;

    private Vector3 targetScale;
    private Color targetColor;

    void Start()
    {
        targetScale = Vector3.one;
        targetColor = textColorNormal;

        if (buttonText != null)
            buttonText.color = textColorNormal;
    }

    void Update()
    {
        // Scale lerp
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleLerpSpeed
        );

        // Color lerp
        if (buttonText != null)
        {
            buttonText.color = Color.Lerp(
                buttonText.color,
                targetColor,
                Time.unscaledDeltaTime * colorLerpSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);

        targetScale = hoverScale;
        targetColor = textColorHighlighted;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one;
        targetColor = textColorNormal;
    }
}