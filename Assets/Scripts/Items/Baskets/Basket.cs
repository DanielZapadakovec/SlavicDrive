using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    public string gatheringPointTag;
    public string pressingMachineTag;

    [Header("Fill Settings")]
    [Range(0f, 1f)]
    public float fullfilment = 0f;

    [Header("UI / Visuals")]
    public GameObject fullfilmentImage;
    public Vector3 startPos;
    public Vector3 endPos;

    public float fillSpeed = 0.001f;
    public float defillSpeed = 0.1f;
    public float lerpSpeed = 5f;
    [Header("Sounds")]
    public AudioSource basketAudioSource;

    [Header("Alcohol Machine")]
    public AlcoholMachine alcoholMachine;

    private void Start()
    {
        if (fullfilmentImage != null)
        {
            fullfilmentImage.transform.localPosition = startPos;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(gatheringPointTag))
        {
            FillingBasket();
        }
         if (other.CompareTag(pressingMachineTag))
        {
            DefillingBasket();
        }
        else
        {
            basketAudioSource.Stop();
        }
    }

    public void FillingBasket()
    {
        if (fullfilment < 1f)
        {
            fullfilment += fillSpeed;
            fullfilment = Mathf.Clamp01(fullfilment);
        }
    }
    public void DefillingBasket()
    {
        if (fullfilment <= 1f && fullfilment >= 0.001f)
        {
            fullfilment -= defillSpeed;
            fullfilment = Mathf.Clamp01(fullfilment);
            
            alcoholMachine.FillingRotatingMachine();
            if (!basketAudioSource.isPlaying)
            {
                basketAudioSource.Play();
            }
        }
    }

    private void Update()
    {
        if (fullfilmentImage != null)
        {
            Vector3 targetPos = Vector3.Lerp(startPos, endPos, fullfilment);
            fullfilmentImage.transform.localPosition = Vector3.Lerp(fullfilmentImage.transform.localPosition, targetPos, Time.deltaTime * lerpSpeed);
        }
    }

    public void SetFullfilmentBasket(float value)
    {
        fullfilment = value;   
    }
}