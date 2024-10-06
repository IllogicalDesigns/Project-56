using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] Button RestartButton;

    SettingsManager settingsManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        settingsManager = FindAnyObjectByType<SettingsManager>();

        // Add listeners to slider events
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });
        fovSlider.onValueChanged.AddListener(delegate { OnFovChanged(); });
        sensSlider.onValueChanged.AddListener(delegate { OnSensChanged(); });

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = settingsManager.volumeValue;

        fovSlider.minValue = settingsManager.minFov;
        fovSlider.maxValue = settingsManager.maxFov;
        fovSlider.value = settingsManager.fovValue;

        sensSlider.minValue = settingsManager.minSens;
        sensSlider.maxValue = settingsManager.maxSens;
        sensSlider.value = settingsManager.sensValue;

        RestartButton.onClick.AddListener(RestartGame);
    }

    public void OnVolumeChanged() {
        settingsManager.OnVolumeChanged(volumeSlider.value);
    }

    public void OnFovChanged() {
        settingsManager.OnFovChanged(fovSlider.value);
    }

    public void OnSensChanged() {
        settingsManager.OnSensChanged(sensSlider.value);
    }

    void RestartGame() {
        FindAnyObjectByType<GameManager>().RestartLevel();
    }
}
