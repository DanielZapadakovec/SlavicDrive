using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
    public enum WeekDay
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    [Header("Time Settings")]
    [Tooltip("Ako r˝chlo plynie Ëas. 60 = 1 min˙ta za sekundu")]
    public float timeMultiplier = 30f;
    private int lastHour;

    [Tooltip("PoËiatoËn˝ Ëas v hre")]
    public int startHour = 8;

    public event Action<int> OnHourChanged;

    [Tooltip("PoËiatoËn˝ deÚ v t˝ûdni")]
    public WeekDay startDay = WeekDay.Monday;

    private DateTime currentTime;
    private int currentDayIndex;

    [Header("Sun Settings")]
    public Light sunLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensityCurve;
    public Transform sunTransform;
    private bool isFastForwarding = false;

    [Tooltip("R˝chlosù ot·Ëania Slnka poËas dÚa")]
    public float sunRotationSpeed = 1f;

    [Header("UI Settings")]
    public Text timeDisplay;
    private DateTime previousTime;

    [Header("Fog Settings")]
    public bool enableFog = true;
    [Tooltip("Fog end distance cez deÚ")]
    public float fogEndDay = 300f;
    [Tooltip("Fog end distance v noci")]
    public float fogEndNight = 80f;
    [Tooltip("Fog farba cez deÚ")]
    public Color fogDayColor = Color.gray;
    [Tooltip("Fog farba v noci")]
    public Color fogNightColor = Color.black;


    private void Start()
    {
        currentTime = DateTime.Today.AddHours(startHour);
        previousTime = currentTime;
        currentDayIndex = (int)startDay;
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
        }
    }

    private void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateLighting();
        UpdateFog();
        UpdateTimeText();

        previousTime = currentTime;

        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        lastHour = currentTime.Hour;

        // Detect crossing midnight
        if (previousTime.Day != currentTime.Day)
        {
            AdvanceDay();
        }
        if (currentTime.Hour != lastHour)
        {
            lastHour = currentTime.Hour;
            OnHourChanged?.Invoke(lastHour);
        }
    }

    private void UpdateTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (currentTime.Hour == 0 && currentTime.Minute == 0 && currentTime.Second < 1f)
        {
            AdvanceDay();
        }
    }

    private void AdvanceDay()
    {
        currentDayIndex = (currentDayIndex + 1) % Enum.GetValues(typeof(WeekDay)).Length;
    }

    private void RotateSun()
    {
        float sunAngle = (float)(currentTime.TimeOfDay.TotalHours / 24f) * 360f;
        sunTransform.rotation = Quaternion.Euler(sunAngle - 90f, -278f, 0f);
    }

    private void UpdateLighting()
    {
        float timePercent = (float)(currentTime.TimeOfDay.TotalHours / 24f);

        sunLight.intensity = lightIntensityCurve.Evaluate(timePercent);
        sunLight.color = lightColor.Evaluate(timePercent);
    }

    private void UpdateTimeText()
    {
        string dayName = ((WeekDay)currentDayIndex).ToString();
        timeDisplay.text = currentTime.ToString("HH:mm") + " | " + dayName;
    }

    public void StartSleepEffect(float fatigue)
    {
        if (!isFastForwarding && fatigue > 0f)
        {
            StartCoroutine(FastForwardSun(fatigue));
        }
    }

    private IEnumerator FastForwardSun(float fatigue)
    {
        isFastForwarding = true;

        float fastForwardDuration = 5f;
        float hoursToSimulate = 7f * fatigue;

        float normalSecondsToSimulate = (hoursToSimulate * 3600f) / timeMultiplier;
        float boostedMultiplier = (hoursToSimulate * 3600f) / fastForwardDuration;

        float originalMultiplier = timeMultiplier;
        timeMultiplier = boostedMultiplier;

        float elapsed = 0f;

        while (elapsed < fastForwardDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        timeMultiplier = originalMultiplier;
        isFastForwarding = false;
    }

    public void SetMultiplier(float value)
    {
        timeMultiplier = value;
    }
    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    private void UpdateFog()
    {
        if (!enableFog) return;

        float timePercent = (float)(currentTime.TimeOfDay.TotalHours / 24f);

        // 0 = noc, 0.5 = poludnie, 1 = noc
        float dayFactor = Mathf.Clamp01(
            lightIntensityCurve.Evaluate(timePercent)
        );

        RenderSettings.fogEndDistance = Mathf.Lerp(
            fogEndNight,
            fogEndDay,
            dayFactor
        );

        RenderSettings.fogColor = Color.Lerp(
            fogNightColor,
            fogDayColor,
            dayFactor
        );
    }


}
