using UnityEngine;
using UnityEngine.UI;

public class WaterFill : MonoBehaviour
{
    public Slider waterSlider;
    public float fillSpeed = 0.25f;

    public bool IsFull => waterSlider.value >= 1f;

    private bool isHolding;

    private void Update()
    {
        if (!isHolding) return;

        waterSlider.value += fillSpeed * Time.deltaTime;
        waterSlider.value = Mathf.Clamp01(waterSlider.value);
    }

    // UI EVENTS
    public void StartFilling() => isHolding = true;
    public void StopFilling() => isHolding = false;

    public void ResetWater()
    {
        waterSlider.value = 0f;
    }
}