using Unity.Cinemachine;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    const string volume = "volume";
    const string fov = "fov";
    const string sens = "sens";

    public float defaultVolume = 0.5f;
    public float volumeValue;

    [Space]
    public float maxFov = 100f;
    public float minFov = 40f;
    public float defaultFov = 75f;
    public float fovValue;
    //[SerializeField] CinemachineCamera virtualCamera;

    [Space]
    public float maxSens = 300f;
    public float minSens = 1f;
    public float defaultSens = 150f;
    public float sensValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        volumeValue = PlayerPrefs.GetFloat(volume, defaultVolume);
        fovValue = PlayerPrefs.GetFloat(fov, defaultFov);
        sensValue = PlayerPrefs.GetFloat(sens, defaultSens);
    }

    public void OnVolumeChanged(float value) {
        Debug.Log($"Volume changed: {value}");
        value = Mathf.Clamp(value, 0f, 1f);
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(volume, value);
        PlayerPrefs.Save();
    }

    public void OnFovChanged(float value) {
        Debug.Log($"FOV changed: {value}");
        value = Mathf.Clamp(value, minFov, maxFov);
        //virtualCamera.Lens.FieldOfView = value;
        PlayerPrefs.SetFloat(fov, Mathf.Clamp(value, minFov, maxFov));
        PlayerPrefs.Save();
    }

    public void OnSensChanged(float value) {
        Debug.Log($"Sensitivity changed: {value}");
        value = Mathf.Clamp(value, minSens, maxSens);
        GameManager.player.rotationSpeed = value;
        PlayerPrefs.SetFloat(sens, value);
        PlayerPrefs.Save();
    }
}
