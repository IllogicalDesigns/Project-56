using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeteactableTargetManager : MonoBehaviour
{
    public static DeteactableTargetManager Instance { get; private set; } = null;

    public List<DetectableTarget> allTargets = new List<DetectableTarget>();

    private void Awake() {
        if (Instance) {
            Debug.LogError("Multiple deteactable manager instances, it should be a singleton, destroying " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(DetectableTarget target) {
        allTargets.Add(target);
    }

    public void DeRegister(DetectableTarget target) {
        allTargets.Remove(target);
    }

}
