using UnityEngine;
using UnityEngine.UI;

public class CrosshairSystem : MonoBehaviour
{
    public enum CrosshairType
    {
        Base,
        Grabbable,
        Interactable,
        Special
    }

    [Header("Crosshairs")]
    [SerializeField] Image[] crosshairs;

    [Header("Settings")]
    [SerializeField] float MaxActiveSize = 90;
    [SerializeField] float MinActiveSize = 20;
    [SerializeField] float Smooth = 10;

    private CrosshairType currentState = CrosshairType.Base;

    void Update()
    {
        for (int i = 0; i < crosshairs.Length; i++)
        {
            Image img = crosshairs[i];

            bool isActive = (i == (int)currentState);

            Color color = img.color;
            color.a = Mathf.Lerp(color.a, isActive ? 1 : 0, Smooth * Time.deltaTime);
            img.color = color;

            float targetSize = isActive ? MaxActiveSize : MinActiveSize;
            float newSize = Mathf.Lerp(img.rectTransform.rect.width, targetSize, Smooth * Time.deltaTime);
            img.rectTransform.sizeDelta = new Vector2(newSize, newSize);
        }
    }

    public void SetCrosshair(CrosshairType type)
    {
        currentState = type;
    }
}