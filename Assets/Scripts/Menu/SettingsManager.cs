using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio - Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio - Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    [Header("Controls")]
    public GameObject controlsPanel;

    // ---- internal ----
    private struct ResOption
    {
        public int w;
        public int h;
        public ResOption(int w, int h) { this.w = w; this.h = h; }
    }

    private List<ResOption> uniqueResolutions = new List<ResOption>();
    private bool ignoreResolutionCallback = false;

    private const float MUTED_DB = -80f;
    private const float MIN_SLIDER_VALUE = 0.0001f;

    // last non-zero values
    private float lastMaster = 1f;
    private float lastMusic = 1f;
    private float lastSfx = 1f;

    private bool isMasterMuted = false;
    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    void Start()
    {
        SetupResolutionOptions();
        LoadSettings();

        // If you didn't wire these in Inspector, this guarantees they work
        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        if (resolutionDropdown)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);

        if (qualityDropdown)
            qualityDropdown.onValueChanged.AddListener(SetQuality);

        if (fullscreenToggle)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    // -------------------------
    // RESOLUTIONS (UNIQUE)
    // -------------------------
    void SetupResolutionOptions()
    {
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();
        uniqueResolutions.Clear();

        // Build unique width/height list
        var seen = new HashSet<string>();
        foreach (var r in Screen.resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (seen.Add(key))
                uniqueResolutions.Add(new ResOption(r.width, r.height));
        }

        // Build dropdown labels
        var options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            var ro = uniqueResolutions[i];
            options.Add($"{ro.w} x {ro.h}");

            if (ro.w == Screen.width && ro.h == Screen.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);

        ignoreResolutionCallback = true;
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
        ignoreResolutionCallback = false;
    }

    void OnResolutionDropdownChanged(int index)
    {
        if (ignoreResolutionCallback) return;
        SetResolution(index);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (uniqueResolutions == null || uniqueResolutions.Count == 0) return;
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, uniqueResolutions.Count - 1);

        var r = uniqueResolutions[resolutionIndex];
        Screen.SetResolution(r.w, r.h, Screen.fullScreen);

        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    // -------------------------
    // AUDIO HELPERS
    // -------------------------
    private void SetMixerVolumeLinear(string param, float sliderValue)
    {
        // 0 => mute in mixer
        if (sliderValue <= 0f)
        {
            audioMixer.SetFloat(param, MUTED_DB);
            return;
        }

        float v = Mathf.Max(MIN_SLIDER_VALUE, sliderValue);
        audioMixer.SetFloat(param, Mathf.Log10(v) * 20f);
    }

    // -------------------------
    // AUDIO - SLIDERS
    // -------------------------
    public void SetMasterVolume(float value)
    {
        if (value > 0f) lastMaster = value;
        isMasterMuted = value <= 0f;

        SetMixerVolumeLinear("MasterVolume", value);
        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.SetInt("masterMuted", isMasterMuted ? 1 : 0);
    }

    public void SetMusicVolume(float value)
    {
        if (value > 0f) lastMusic = value;
        isMusicMuted = value <= 0f;

        SetMixerVolumeLinear("MusicVolume", value);
        PlayerPrefs.SetFloat("musicVolume", value);
        PlayerPrefs.SetInt("musicMuted", isMusicMuted ? 1 : 0);
    }

    public void SetSfxVolume(float value)
    {
        if (value > 0f) lastSfx = value;
        isSfxMuted = value <= 0f;

        SetMixerVolumeLinear("SFXVolume", value);
        PlayerPrefs.SetFloat("sfxVolume", value);
        PlayerPrefs.SetInt("sfxMuted", isSfxMuted ? 1 : 0);
    }

    // -------------------------
    // CLICK-TO-MUTE (TEXT BUTTONS)
    // Sets slider to 0 and restores previous value on second click.
    // -------------------------
    public void ToggleMasterMute()
    {
        ToggleChannelMute(ref isMasterMuted, masterSlider, ref lastMaster, "MasterVolume", "masterMuted");
    }

    public void ToggleMusicMute()
    {
        ToggleChannelMute(ref isMusicMuted, musicSlider, ref lastMusic, "MusicVolume", "musicMuted");
    }

    public void ToggleSfxMute()
    {
        ToggleChannelMute(ref isSfxMuted, sfxSlider, ref lastSfx, "SFXVolume", "sfxMuted");
    }

    private void ToggleChannelMute(ref bool isMuted, Slider slider, ref float lastValue, string mixerParam, string prefsMutedKey)
    {
        if (slider == null) return;

        if (!isMuted)
        {
            // MUTING
            if (slider.value > 0f)
                lastValue = slider.value;
            if (lastValue <= 0f)
                lastValue = 1f; // safe default

            isMuted = true;

            slider.value = 0f;                // will call SetXVolume via listener
            audioMixer.SetFloat(mixerParam, MUTED_DB); // ensure immediate

            PlayerPrefs.SetInt(prefsMutedKey, 1);
        }
        else
        {
            // UNMUTING
            isMuted = false;

            float restore = Mathf.Max(MIN_SLIDER_VALUE, lastValue);
            slider.value = restore;           // will call SetXVolume via listener
            SetMixerVolumeLinear(mixerParam, restore);

            PlayerPrefs.SetInt(prefsMutedKey, 0);
        }
    }

    // -------------------------
    // GRAPHICS
    // -------------------------
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
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

    // -------------------------
    // LOAD
    // -------------------------
    void LoadSettings()
    {
        // AUDIO
        float master = PlayerPrefs.GetFloat("masterVolume", 1f);
        float music = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 1f);

        lastMaster = Mathf.Max(MIN_SLIDER_VALUE, master);
        lastMusic = Mathf.Max(MIN_SLIDER_VALUE, music);
        lastSfx = Mathf.Max(MIN_SLIDER_VALUE, sfx);

        isMasterMuted = PlayerPrefs.GetInt("masterMuted", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt("sfxMuted", 0) == 1;

        if (masterSlider) masterSlider.value = isMasterMuted ? 0f : master;
        if (musicSlider) musicSlider.value = isMusicMuted ? 0f : music;
        if (sfxSlider) sfxSlider.value = isSfxMuted ? 0f : sfx;

        // apply to mixer immediately
        SetMixerVolumeLinear("MasterVolume", isMasterMuted ? 0f : master);
        SetMixerVolumeLinear("MusicVolume", isMusicMuted ? 0f : music);
        SetMixerVolumeLinear("SFXVolume", isSfxMuted ? 0f : sfx);

        // GRAPHICS
        if (PlayerPrefs.HasKey("quality"))
        {
            int q = PlayerPrefs.GetInt("quality");
            if (qualityDropdown) qualityDropdown.value = q;
            SetQuality(q);
        }

        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool fs = PlayerPrefs.GetInt("fullscreen") == 1;
            if (fullscreenToggle) fullscreenToggle.isOn = fs;
            SetFullscreen(fs);
        }

        if (PlayerPrefs.HasKey("resolution"))
        {
            int resIndex = PlayerPrefs.GetInt("resolution");
            resIndex = Mathf.Clamp(resIndex, 0, Mathf.Max(0, uniqueResolutions.Count - 1));

            if (resolutionDropdown)
            {
                ignoreResolutionCallback = true;
                resolutionDropdown.value = resIndex;
                resolutionDropdown.RefreshShownValue();
                ignoreResolutionCallback = false;
            }

            SetResolution(resIndex);
        }
    }
}