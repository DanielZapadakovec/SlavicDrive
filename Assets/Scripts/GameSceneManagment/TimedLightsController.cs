using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimedLightsController : MonoBehaviour
{
    [Header("References")]
    public DayNightCycle dayNightCycle;
    public List<Light> lights = new List<Light>();

    [Header("Time Settings")]
    [Tooltip("Kedy sa maj˙ svetl· zapÌnaù (hodina)")]
    public int turnOnHour = 18;

    [Tooltip("Kedy sa maj˙ svetl· vypÌnaù (hodina)")]
    public int turnOffHour = 6;

    [Header("Switch Settings")]
    [Tooltip("Interval medzi zapnutÌm jednotliv˝ch svetiel")]
    public float switchInterval = 0.2f;

    private bool lightsOn = false;
    private Coroutine switchRoutine;

    private void Start()
    {
        SetAllLights(false);
    }

    private void Update()
    {
        if (dayNightCycle == null) return;

        DateTime time = dayNightCycle.GetCurrentTime();
        int hour = time.Hour;

        bool shouldBeOn = IsNightTime(hour);

        if (shouldBeOn && !lightsOn)
        {
            StartSwitching(true);
        }
        else if (!shouldBeOn && lightsOn)
        {
            StartSwitching(false);
        }
    }

    private bool IsNightTime(int hour)
    {
        // napr. 18:00 ñ 06:00
        if (turnOnHour < turnOffHour)
            return hour >= turnOnHour && hour < turnOffHour;
        else
            return hour >= turnOnHour || hour < turnOffHour;
    }

    private void StartSwitching(bool turnOn)
    {
        if (switchRoutine != null)
            StopCoroutine(switchRoutine);

        switchRoutine = StartCoroutine(SwitchLightsSequentially(turnOn));
        lightsOn = turnOn;
    }

    private IEnumerator SwitchLightsSequentially(bool turnOn)
    {
        foreach (Light l in lights)
        {
            if (l != null)
                l.enabled = turnOn;

            yield return new WaitForSeconds(switchInterval);
        }
    }

    private void SetAllLights(bool state)
    {
        foreach (Light l in lights)
        {
            if (l != null)
                l.enabled = state;
        }
    }
}
