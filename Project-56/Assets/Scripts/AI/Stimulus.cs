using UnityEngine;

public class Stimulus : MonoBehaviour
{
    public float awareness;
    public int priority = 1;
    public float maxAwareness = 3f;

    bool increasedThisFrame;

    private void Update() {
        if (increasedThisFrame) {
            increasedThisFrame = false;
            return;
        }

        if (awareness >= 0) awareness -= Time.deltaTime;
    }

    public void AdjustStim(float adjustment) {
        increasedThisFrame = true;

        awareness = Mathf.Clamp(awareness + adjustment, 0f, maxAwareness);
    }

    public void UpdateStim(float _awareness) {
        var cat = FindAnyObjectByType<Cat>();

        awareness = _awareness;
        awareness = Mathf.Clamp(awareness, 0f, maxAwareness);

        if (cat != null) {
            cat.SoundStimuli(this);
        }
    }
}
