using DG.Tweening;
using UnityEngine;

public class PlayerCameraLeaning : MonoBehaviour {
    public const KeyCode leanLeftKeyName = KeyCode.Q;
    public const KeyCode leanRightKeyName = KeyCode.E;

    [SerializeField] private Ease leanEase = Ease.OutQuad;
    [SerializeField] Transform cameraHolder;
    [SerializeField] float leanLeftMax = 15f;
    [SerializeField] float leanRightMax = -15f;
    [SerializeField] float leanSpeed = 0.2f;

    private Tween leanTween;

    void Update() {
        if (Input.GetKey(leanLeftKeyName)) {
            Lean(leanLeftMax);
        }
        else if (Input.GetKey(leanRightKeyName)) {
            Lean(leanRightMax);
        }
        else {
            Lean(0f); // Lean back to center
        }
    }

    private void Lean(float targetZRotation) {
        // If there's an active tween, kill it before creating a new one
        if (leanTween != null && leanTween.IsActive()) {
            leanTween.Kill();
        }

        // Start a new tween to the target rotation
        leanTween = cameraHolder.DOLocalRotate(new Vector3(cameraHolder.localEulerAngles.x, cameraHolder.localEulerAngles.y, targetZRotation), leanSpeed).SetEase(leanEase);
    }
}
