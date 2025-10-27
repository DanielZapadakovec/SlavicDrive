using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlcoholMachine : MonoBehaviour
{

    [Header("Fill Settings")]
    [Range(0f, 1f)]
    public float rotatingFullfilment = 0f;
    [Header("RotatingMachine")]
    public GameObject rotatingFullfilmentImage;
    public Vector3 startPos;
    public Vector3 endPos;
    public float rotatingFillSpeed = 0.01f;
    public float rotatingDefillSpeed = 0.1f;
    public float rotatingLerpSpeed = 5f;
    public GameObject rotatingMachine;
    bool isRotating;
    public AudioSource rotatingSound;
    public AudioClip machineRotatingClip;

    [Header("PreparingMachine")]
    public GameObject heaterFullfilmentImage;
    public Vector3 heaterStartPos;
    public Vector3 heaterEndPos;
    public float preparingFillSpeed = 0.0001f;
    public float preparingDefillSpeed = 0.0001f;
    public float preparingLerpSpeed = 5f;
    public AudioSource preparingSound;
    public AudioClip fillingHeaterClip;
    public bool isLevered;
    public Animator leverAnimator;
    [Header("Heater Fill Settings")]
    [Range(0f, 1f)]
    public float preparingFullfilment = 0f;

    [Header("Alcohol Making")]
    public HeaterMachine heaterMachine;
    public float alcoholAmount = 0f;
    public float alcoholFillSpeed = 0.003f;
    public Text alcoholMachineUIText;

    [Header("Special_Effects")]
    public GameObject heatDistortionEffect;




    #region RotatingMachine
    public void FillingRotatingMachine()
    {
        if (rotatingFullfilment < 1f)
        {
            rotatingFullfilment += rotatingFillSpeed;
            rotatingFullfilment = Mathf.Clamp01(rotatingFullfilment);
        }
    }
    public void DefillingRotatingMachine()
    {
        if (rotatingFullfilment <= 1f || rotatingFullfilment >= 0.001f  && preparingFullfilment < 1)
        {
            rotatingFullfilment -= rotatingDefillSpeed;
            rotatingFullfilment = Mathf.Clamp01(rotatingFullfilment);

        }
    }
    public void PressingRotatingSwitch()
    {
        if (!isRotating)
        {
            isRotating = true;
        }
        else if (isRotating)
        {
            isRotating = false;
        }
    }
    #endregion
    #region PreparingMachine
    public void FillingPreparingMachine()
    {
        if (preparingFullfilment < 1f)
        {
            preparingFullfilment += preparingFillSpeed;
            preparingFullfilment = Mathf.Clamp01(preparingFullfilment);
        }
    }
    public void DefillingPreparingMachine()
    {
        if (preparingFullfilment < 1f || rotatingFullfilment > 0.001f)
        {
            preparingFullfilment -= preparingDefillSpeed;
            preparingFullfilment = Mathf.Clamp01(preparingFullfilment);

        }
    }
    public void PreparingLever()
    {
        if (!isLevered)
        {
            isLevered = true;
            leverAnimator.SetBool("isInteracting", true);
        }
        else if (isLevered)
        {
            isLevered = false;
            leverAnimator.SetBool("isInteracting", false);
        }
    }
    #endregion

    private void Update()
    {
        if (rotatingFullfilmentImage != null)
        {
            Vector3 targetPos = Vector3.Lerp(startPos, endPos, rotatingFullfilment);
            rotatingFullfilmentImage.transform.localPosition = Vector3.Lerp(rotatingFullfilmentImage.transform.localPosition, targetPos, Time.deltaTime * rotatingLerpSpeed);
            if (isRotating && rotatingFullfilment > 0.01f)
            {
                rotatingMachine.transform.Rotate(0f, 10f * Time.deltaTime, 0f, Space.Self);
                if(!rotatingSound.isPlaying)
                {
                    rotatingSound.clip = machineRotatingClip;
                    rotatingSound.Play();
                }
                if(rotatingFullfilment <=1f && rotatingFullfilment > 0.0001f && preparingFullfilment <=1f)
                {
                    DefillingRotatingMachine();
                    FillingPreparingMachine();
                }
            }
            else
            {
                rotatingMachine.transform.Rotate(0f, 0f, 0f, Space.Self);
                isRotating = false;
                rotatingSound.Stop();
            }
        }
        if (heaterFullfilmentImage != null)
        {
            Vector3 targetPos = Vector3.Lerp(heaterStartPos, heaterEndPos, preparingFullfilment);
            heaterFullfilmentImage.transform.localPosition = Vector3.Lerp(heaterFullfilmentImage.transform.localPosition, targetPos, Time.deltaTime * rotatingLerpSpeed);
        }
        if (preparingFullfilment > 0.001f && heaterMachine.temperature >= 80f && heaterMachine.temperature <= 140f && alcoholAmount <1)
        {
            alcoholAmount += alcoholFillSpeed * Time.deltaTime;
            DefillingPreparingMachine();
            alcoholAmount = Mathf.Clamp01(alcoholAmount);
            heatDistortionEffect.SetActive(true);
        }
        else if ( heaterMachine.temperature < 80f)
        {
            heatDistortionEffect.SetActive(false);
        }

        if (alcoholMachineUIText != null)
        {
            int percent = Mathf.RoundToInt(alcoholAmount * 100f);
            alcoholMachineUIText.text = $"Alcohol: {percent}%";
        }

    }

    public void SetAlcoholAmountFull(float value)
    {
        alcoholAmount = value;
    }

}
