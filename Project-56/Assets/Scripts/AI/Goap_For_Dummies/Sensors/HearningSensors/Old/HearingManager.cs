using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour {
    public static HearingManager Instance { get; private set; } = null;  // read-only, internally setable

    public List<HearingSensor> allSensors = new List<HearingSensor>();

    public enum EHeardSoundCategory {
        EFootStep,
        EJump,
        EGunShot,
        EImportant
    }

    private void Awake() {
        if (Instance) {
            Debug.LogError("Multiple hearing manager instances, it should be a singleton, destroying " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(HearingSensor sensor) {
        allSensors.Add(sensor);
    }

    public void DeRegister(HearingSensor sensor) {
        allSensors.Remove(sensor);
    }

    public void OnSoundEmitted(GameObject source, HearingManager.EHeardSoundCategory category, float intensity) {
        foreach (var sensor in allSensors) {
            //sensor.OnHeardSound(source, category, intensity);
        }
    }
}
