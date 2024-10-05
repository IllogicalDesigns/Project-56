using UnityEngine;

public interface IVisionSensor {
    float CanWeSeeTarget(GameObject candidateGameObject);
}