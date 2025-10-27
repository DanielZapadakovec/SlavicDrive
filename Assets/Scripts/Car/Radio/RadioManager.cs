

using NAudio.Wave;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages radio functionality, including volume control and channel selection.
/// </summary>
public class RadioManager : MonoBehaviour
{

    public Slider volumeSlider;

    public TMP_Dropdown radioDropdown;

    public string[] icecastUrls = {
        "https://stream.zeno.fm/7gi0z5uqrzbvv?t=1742377991169",
        "https://icecast6.play.cz/cesky-impuls.mp3",
        "https://stream.bauermedia.sk/europa2-lo.mp3",
        "https://stream.bauermedia.sk/melody-lo.mp3",
        "https://stream.bauermedia.sk/rock-lo.mp3"

    };

    private MediaFoundationReader mediaFoundationReader;

    private WaveOutEvent waveOut;
    public AudioSource audioSource;

    void Start()
    {
        SetVolume(0.1f);

    }

    public IEnumerator PlayRadio(string url)
    {
        yield return null;
        try
        {
            mediaFoundationReader = new MediaFoundationReader(url);
            waveOut = new WaveOutEvent();
            waveOut.Init(mediaFoundationReader);
            waveOut.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error playing radio: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
        }

        if (mediaFoundationReader != null)
        {
            mediaFoundationReader.Dispose();
        }
    }

    public void StopRadio()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        if (waveOut != null)
        {
            waveOut.Volume = Mathf.Clamp01(volume);
        }
    }

    public float GetVolume()
    {
        return waveOut != null ? waveOut.Volume : 0;
    }

}