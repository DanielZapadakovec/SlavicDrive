using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    public List<GameObject> achievements = new List<GameObject>();

    public float fadeDuration = 0.5f;
    public float showTime = 3f;
    public AudioSource achievementAudioSource;
    public AudioClip achievementClip;

    [Header("CarAssembly")]
    public CarAssembly carAssembly;
    bool wheel;
    bool battery;
    bool seat;



    public void ShowAchievement(int id)
    {
        if (id < 0 || id >= achievements.Count) return;

        StartCoroutine(ShowAchievementRoutine(achievements[id]));
    }

    IEnumerator ShowAchievementRoutine(GameObject obj)
    {
        obj.SetActive(true);

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        // RESET
        cg.alpha = 0f;
        obj.transform.localScale = Vector3.zero;

        // FADE IN + SCALE
        float t = 0;
        achievementAudioSource.PlayOneShot(achievementClip);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            cg.alpha = Mathf.Lerp(0f, 1f, lerp);
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerp);

            yield return null;
        }

        cg.alpha = 1f;
        obj.transform.localScale = Vector3.one;

        // WAIT
        yield return new WaitForSeconds(showTime);

        // FADE OUT
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            cg.alpha = Mathf.Lerp(1f, 0f, lerp);
            obj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, lerp);

            yield return null;
        }

        obj.SetActive(false);
    }

    public void Update()
    {
        if (carAssembly.IsPartInstalled(ItemType.SteeringWheel) && wheel)
        {
            ShowAchievement(1);
            wheel = true;
        }
        else if (carAssembly.IsPartInstalled(ItemType.Seat) && seat)
        {
            ShowAchievement(2);
            seat = true;
        }
        else if (carAssembly.IsPartInstalled(ItemType.Battery) && battery)
        {
            ShowAchievement(3);
            battery = true;
        }
    }
}