using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Ako rýchlo plynie èas. 60 = 1 minúta za sekundu")]
    public float timeMultiplier = 30f;

    [Tooltip("Poèiatoèný èas v hre")]
    public int startHour = 8;

    private DateTime currentTime;

    [Header("Sun Settings")]
    public Light sunLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensityCurve;
    public Transform sunTransform;
    private bool isFastForwarding = false;

    [Tooltip("Rýchlos otáèania Slnka poèas dòa")]
    public float sunRotationSpeed = 1f;

    [Header("UI Settings")]
    public Text timeDisplay;


    private void Start()
    {
        currentTime = DateTime.Today.AddHours(startHour);
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
    }

    private void RotateSun()
    {
        float sunAngle = (float)(currentTime.TimeOfDay.TotalHours / 24f) * 360f;
        sunTransform.rotation = Quaternion.Euler(sunAngle - 90f, -278f, 0f); // -90 posúva slnko aby zaèínalo na východe
    }

    private void UpdateLighting()
    {
        float timePercent = (float)(currentTime.TimeOfDay.TotalHours / 24f);

        sunLight.intensity = lightIntensityCurve.Evaluate(timePercent);
        sunLight.color = lightColor.Evaluate(timePercent);
    }

    private void UpdateTimeText()
    {
        timeDisplay.text = currentTime.ToString("HH:mm");
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

        float fastForwardDuration = 5f; // v sekundách
        float hoursToSimulate = 7f * fatigue;

        // Ko¾ko sekúnd v hre treba aby prebehlo hoursToSimulate hodín
        float normalSecondsToSimulate = (hoursToSimulate * 3600f) / timeMultiplier;

        // Potrebujeme nový timeMultiplier taký, aby sa simulovaný èas prebehol za fastForwardDuration
        float boostedMultiplier = (hoursToSimulate * 3600f) / fastForwardDuration;

        float originalMultiplier = timeMultiplier;
        timeMultiplier = boostedMultiplier;

        float elapsed = 0f;

        while (elapsed < fastForwardDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Po skonèení efektu vrátime èasový multiplikátor spä
        timeMultiplier = originalMultiplier;
        isFastForwarding = false;
    }

    public void SetMultiplier(float value)
    {
        timeMultiplier = value;
    }

}
