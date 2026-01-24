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
    public GameObject Canvas;

    public AudioMixer mixer;

    public void SetLowpass(float value)
    {
        // value 0–1 → Hz
        float cutoff = Mathf.Lerp(22000f, 800f, value);
        mixer.SetFloat("Bus_Lowpass", cutoff);
    }

    public void SetDistortion(float value)
    {
        mixer.SetFloat("Bus_Distortion", Mathf.Lerp(0f, 0.7f, value));
    }

    public void SetReverb(float value)
    {
        mixer.SetFloat("Bus_Reverb", Mathf.Lerp(-10000f, 0f, value));
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
        Canvas.SetActive(true);
    }
}
