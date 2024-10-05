using UnityEngine;

public interface IHearingSensor {
    bool CanWeHear(GameObject candidateAudioSource, float overrideHearingDist = 0);
}
