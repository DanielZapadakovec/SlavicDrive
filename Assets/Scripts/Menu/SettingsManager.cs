using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    [Header("Graphics")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    [Header("Controls")]
    public GameObject controlsPanel;

    private Resolution[] resolutions;

    void Start()
    {
        SetupResolutionOptions();
        LoadSettings();
    }

    void SetupResolutionOptions()
    {
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void ShowControlsPanel(bool state)
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(state);
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            volumeSlider.value = volume;
            SetVolume(volume);
        }

        if (PlayerPrefs.HasKey("quality"))
        {
            int quality = PlayerPrefs.GetInt("quality");
            qualityDropdown.value = quality;
            SetQuality(quality);
        }

        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            fullscreenToggle.isOn = fullscreen;
            SetFullscreen(fullscreen);
        }

        if (PlayerPrefs.HasKey("resolution"))
        {
            int resIndex = PlayerPrefs.GetInt("resolution");
            resolutionDropdown.value = resIndex;
            SetResolution(resIndex);
        }
    }
}
