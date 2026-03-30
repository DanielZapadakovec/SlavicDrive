using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class BusTimelineMethods : MonoBehaviour
{
    public AudioSource source;
    public float blendSpeed = 2f;
    public GameObject Player;
    public Volume volume;
    public VolumeProfile normalProfile;
    public GameObject MainCanvas;
    public GameObject secondCanvas;


    public AudioMixer mixer;
    public void Start()
    {
        Player.gameObject.SetActive(false);
    }

    public void SetLowpass(float value)
    {
        StartCoroutine(AnimateLowpass(5,value));
    }
    public void SetHighPass(float value)
    {
        // value 0–1 → Hz
        float cutoff = Mathf.Lerp(800f, 2500f, value);
        mixer.SetFloat("Bus_Highpass", cutoff);
    }

    public IEnumerator AnimateLowpass(float targetValue, float duration)
    {
        float startValue;
        mixer.GetFloat("Bus_Lowpass", out startValue);

        float startTime = 0f;

        float targetCutoff = Mathf.Lerp(22000f, 3500f, targetValue);

        while (startTime < duration)
        {
            startTime += Time.deltaTime;
            float t = startTime / duration;

            float current = Mathf.Lerp(startValue, targetCutoff, t);
            mixer.SetFloat("Bus_Lowpass", current);

            yield return null;
        }

        mixer.SetFloat("Bus_Lowpass", targetCutoff);
    }

    public void SetSpatial(float target)
    {
        StopAllCoroutines();
        StartCoroutine(BlendSpatial(target));
    }

    private IEnumerator BlendSpatial(float target)
    {
        while (Mathf.Abs(source.spatialBlend - target) > 0.01f)
        {
            source.spatialBlend = Mathf.Lerp(
                source.spatialBlend,
                target,
                Time.deltaTime * blendSpeed
            );
            yield return null;
        }
        source.spatialBlend = target;
    }

    public void EnablePlayer()
    {
        Player.SetActive(true);
        volume.profile = normalProfile;
        MainCanvas.SetActive(true);
        secondCanvas.SetActive(false);
    }
}
