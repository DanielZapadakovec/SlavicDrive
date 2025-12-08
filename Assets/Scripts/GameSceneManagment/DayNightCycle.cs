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
    [Tooltip("Ako rıchlo plynie èas. 60 = 1 minúta za sekundu")]
    public float timeMultiplier = 30f;

    [Tooltip("Poèiatoènı èas v hre")]
    public int startHour = 8;

    [Tooltip("Poèiatoènı deò v tıdni")]
    public WeekDay startDay = WeekDay.Monday;

    private DateTime currentTime;
    private int currentDayIndex;

    [Header("Sun Settings")]
    public Light sunLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensityCurve;
    public Transform sunTransform;
    private bool isFastForwarding = false;

    [Tooltip("Rıchlos otáèania Slnka poèas dòa")]
    public float sunRotationSpeed = 1f;

    [Header("UI Settings")]
    public Text timeDisplay;


    private void Start()
    {
        currentTime = DateTime.Today.AddHours(startHour);
        currentDayIndex = (int)startDay;
    }

    private void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateLighting();
        UpdateTimeText();
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
}
