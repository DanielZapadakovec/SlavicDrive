using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaterMachine : MonoBehaviour
{
    [Header("HeaterLogic")]
    public ItemGrabber grabObject;
    Rigidbody objectBody;
    ItemID grabbableItemID;
    Outline outline;
   // Collider objectCollider;
    GameObject log;
    public List<GameObject> logList;
    bool isCatching;

    [Header("TemperatureLogic")]
    public GameObject fireEffect;
    public float temperature = 0f;
    public float maxTemperature = 100f;
    public float optimalLogCount = 3f;
    public float temperaturePerLog = 70f;
    [Header("TemperatureNeedle")]
    public Transform temperatureNeedle;
    public float needleMinRotation = -90f;
    public float needleMaxRotation = 90f;
    public float logBurnRate = 0.01f;
    float currentNeedleAngle = 0f;
    float needleVelocity = 0f;
    public float needleSmoothTime = 0.5f;
    [Header("Sounds")]
    public AudioSource heaterAudioSource;
    bool isIgnited = false;
    #region HeaterLogic
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Log"))
        {
            if (!isCatching)
            {
                log = other.gameObject;
                StartCoroutine(CatchingLog());
                log.tag = "Untagged";
                log.layer = 0;
                isCatching = true;
            }


        }
    }

    public IEnumerator CatchingLog()
    {
        yield return new WaitForSeconds(1);
        objectBody = log.GetComponent<Rigidbody>();
        grabbableItemID = log.GetComponent<ItemID>();
        outline = log.GetComponent<Outline>();
      //  objectCollider = log.GetComponent<Collider>();
        grabbableItemID.enabled = false;
        outline.enabled = false;
        Destroy(objectBody);
      //  objectCollider.enabled = false;
        logList.Add(log);
        log = null;
        isCatching = false;
        yield break;
    }
    #endregion
    #region TemperatureLogic
    public void Ignite()
    {
        if (!isIgnited && logList.Count > 0)
        {
            isIgnited = true;
            fireEffect.SetActive(true);
        }
    }

    void Update()
    {
        if (isIgnited && logList.Count > 0)
        {
            UpdateTemperature();
            BurnLogs();
            if (!heaterAudioSource.isPlaying)
            {
                heaterAudioSource.Play();
            }
        }
        else
        {
            heaterAudioSource.Stop();
            fireEffect.SetActive(false);
        }
        UpdateNeedle();
        
    }

    void UpdateTemperature()
    {
        float totalTemp = 0f;
        foreach (var log in logList)
        {
            if (log != null)
            {
                float scaleFactor = log.transform.localScale.y /3f;
                totalTemp += scaleFactor * temperaturePerLog;
            }
        }

        temperature = Mathf.Clamp(totalTemp, 0, maxTemperature);
    }
    void BurnLogs()
    {
        for (int i = logList.Count - 1; i >= 0; i--)
        {
            GameObject log = logList[i];
            if (log != null)
            {
                Vector3 scale = log.transform.localScale;
                scale -= Vector3.one * logBurnRate * Time.deltaTime;
                scale = Vector3.Max(scale, Vector3.zero);
                log.transform.localScale = scale;

                if (scale.magnitude <= 0.1f)
                {
                    Destroy(log);
                    logList.RemoveAt(i);
                }
            }
            else
            {
                fireEffect.SetActive(false);
            }
        }
    }

    void UpdateNeedle()
    {
        float t = temperature / maxTemperature;
        float targetAngle = Mathf.Lerp(needleMinRotation, needleMaxRotation, t);

        currentNeedleAngle = Mathf.SmoothDampAngle(currentNeedleAngle, targetAngle, ref needleVelocity, needleSmoothTime);

        temperatureNeedle.localRotation = Quaternion.Euler(currentNeedleAngle, 0, 0);
    }
    #endregion

    public void SetTemperatureTo120(float value)
    {
        temperature = value;
    }
}
