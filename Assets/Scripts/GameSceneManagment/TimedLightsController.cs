using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimedLightsController : MonoBehaviour
{
    [Header("References")]
    public DayNightCycle dayNightCycle;

    private List<Light> lights = new List<Light>();
    private List<Renderer> renderers = new List<Renderer>();
    private List<LensFlare> lensFlares = new List<LensFlare>();

    [Header("Materials")]
    public Material lightOffMaterial;
    public Material lightOnMaterial;

    [Tooltip("Index materiálu (napr. 1 = druhý materiál)")]
    public int materialIndex = 1;

    [Header("Time Settings")]
    public int turnOnHour = 18;
    public int turnOffHour = 6;

    [Header("Switch Settings")]
    public float switchInterval = 0.2f;

    private bool lightsOn = false;
    private Coroutine switchRoutine;

    private void Start()
    {
        FindStreetLights();
        SetAllLights(false);
    }

    private void FindStreetLights()
    {
        lights.Clear();
        renderers.Clear();
        lensFlares.Clear();

        GameObject[] lamps = GameObject.FindGameObjectsWithTag("StreetLamp");

        foreach (GameObject lamp in lamps)
        {
            // Renderer (na materiál)
            Renderer rend = lamp.GetComponentInChildren<Renderer>();
            if (rend != null)
                renderers.Add(rend);

            // Light
            Light lightComponent = lamp.GetComponentInChildren<Light>();
            if (lightComponent != null)
            {
                lights.Add(lightComponent);

                // Lens Flare
                LensFlare flare = lightComponent.GetComponent<LensFlare>();
                lensFlares.Add(flare); // môže byť null
            }
        }
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
        for (int i = 0; i < lights.Count; i++)
        {
            // 💡 Light
            if (lights[i] != null)
                lights[i].enabled = turnOn;

            // 🎨 Material (len konkrétny index!)
            if (i < renderers.Count && renderers[i] != null)
            {
                Renderer rend = renderers[i];
                Material[] mats = rend.materials;

                if (materialIndex < mats.Length)
                {
                    mats[materialIndex] = turnOn ? lightOnMaterial : lightOffMaterial;
                    rend.materials = mats;
                }
            }

            // ✨ Lens Flare
            if (i < lensFlares.Count && lensFlares[i] != null)
                lensFlares[i].enabled = turnOn;

            yield return new WaitForSeconds(switchInterval);
        }
    }

    private void SetAllLights(bool state)
    {
        for (int i = 0; i < lights.Count; i++)
        {
            if (lights[i] != null)
                lights[i].enabled = state;

            if (i < renderers.Count && renderers[i] != null)
            {
                Renderer rend = renderers[i];
                Material[] mats = rend.materials;

                if (materialIndex < mats.Length)
                {
                    mats[materialIndex] = state ? lightOnMaterial : lightOffMaterial;
                    rend.materials = mats;
                }
            }

            if (i < lensFlares.Count && lensFlares[i] != null)
                lensFlares[i].enabled = state;
        }
    }
}