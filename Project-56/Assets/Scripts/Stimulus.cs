using UnityEngine;

public class Stimulus : MonoBehaviour
{
    public float awareness;
    public int priority = 1;
    public float maxAwareness = 3f;

    private void Update() {
        if(awareness >= 0) awareness -= Time.deltaTime;
    }

    public void UpdateStim(float _awareness) {
        var cat = FindAnyObjectByType<Cat>();

        if(cat != null) {
            cat.SoundStimuli(this);
        }

        awareness = Mathf.Clamp(_awareness, 0f, maxAwareness);
    }
}
