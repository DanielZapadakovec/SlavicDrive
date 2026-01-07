using UnityEngine;
using UnityEngine.UI;

public class TemperatureMiniGame : MonoBehaviour
{
    [Header("Temperature")]
    public float temperature = 20f;
    public float minTemp = 20f;
    public float maxTemp = 150f;
    public float changeStep = 5f;

    [Header("Cooling")]
    public float coolingSpeed = 3f;

    [Header("Cooking Time")]
    public float cookingTime = 30f;
    private float timer;

    [Header("UI")]
    public Text temperatureText;
    public Slider timeSlider;

    public bool IsFinished => timer >= cookingTime;

    private void OnEnable()
    {
        timer = 0f;
        UpdateUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timeSlider.value = timer / cookingTime;
        temperature -= coolingSpeed * Time.deltaTime;
        temperature = Mathf.Clamp(temperature, minTemp, maxTemp);
        UpdateUI();
    }

    public void IncreaseTemp()
    {
        temperature = Mathf.Clamp(temperature + changeStep, minTemp, maxTemp);
        UpdateUI();
    }

    public void DecreaseTemp()
    {
        temperature = Mathf.Clamp(temperature - changeStep, minTemp, maxTemp);
        UpdateUI();
    }

    private void UpdateUI()
    {
        temperatureText.text = Mathf.RoundToInt(temperature) + " °C";
    }
}
