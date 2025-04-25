using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float maxVolumeLevel = 1.0f;
    private const string VOLUME_PREFS = "GameVolume";
    private float currentVolume;

    void Start()
    {
        float defaultVolume = maxVolumeLevel * 0.5f;
        currentVolume = PlayerPrefs.GetFloat(VOLUME_PREFS, defaultVolume);

        // Configure slider
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = currentVolume / maxVolumeLevel; // Normalize to slider range

        // Apply initial volume
        SetVolumeFromSlider(volumeSlider.value);

        // Add listener for slider changes
        volumeSlider.onValueChanged.AddListener(SetVolumeFromSlider);
    }

    void OnEnable()
    {
        // Restore volume settings when the options menu opens
        if (volumeSlider != null)
        {
            volumeSlider.value = currentVolume / maxVolumeLevel;
        }
    }

    public void SetVolumeFromSlider(float sliderValue)
    {
        // Convert slider value (0-1) to capped volume range (0-maxVolumeLevel)
        currentVolume = Mathf.Clamp(sliderValue * maxVolumeLevel, 0f, maxVolumeLevel);

        // Set master volume
        AudioListener.volume = currentVolume;

        // Save current volume
        PlayerPrefs.SetFloat(VOLUME_PREFS, currentVolume);
        PlayerPrefs.Save();

        Debug.Log($"Volume set to: {currentVolume:F2} ({Mathf.Round(sliderValue * 100)}%)");
    }
    public float GetCurrentVolume()
    {
        return currentVolume;
    }
}