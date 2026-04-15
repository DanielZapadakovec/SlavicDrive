using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BusTimelineMethods : MonoBehaviour
{
    public AudioSource source;
    public float blendSpeed = 2f;
    public GameObject Player;
    public Volume volume;
    public VolumeProfile normalProfile;
    public GameObject MainCanvas;
    public GameObject secondCanvas;
    public GameObject bus;
    [Header("SkipScene")]
    public CarAIFollower follower;
    public SplineContainer secondSpline;
    public Vector3 busSkippedPosition;
    public Quaternion busSkippedRotation;
    public PlayableDirector busTimeline;
    private float holdTime = 0f;
    public float requiredHoldTime = 3f;
    public Image spaceFillImage;
    public AudioMixer mixer;

    public bool canSkip;
    public void Start()
    {
        Player.SetActive(false);
        canSkip = true;
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.Space) && canSkip)
        {
            float fill = Mathf.Clamp01(holdTime / requiredHoldTime);
            spaceFillImage.fillAmount = fill;
            holdTime += Time.deltaTime;
            if (holdTime >= requiredHoldTime)
            {
                SkipScene();
            }
        }
        else
        {
            holdTime = 0f;
            spaceFillImage.fillAmount = 0;
        }
    }

    public void SetLowpass(float value)
    {
        StartCoroutine(AnimateLowpass(5, value));
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

    public void SkipScene()
    {
        bus.transform.position = busSkippedPosition;
        bus.transform.rotation = busSkippedRotation;
        follower.SetSecondSplineActive(secondSpline);
        busTimeline.time = 106f;
        canSkip = false;
    }
}
