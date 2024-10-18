using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TMP_FontAsset fontAsset;
    [SerializeField] TMP_FontAsset enteredFontAsset;

    [Space]
    public Vector3 onEnterSize = Vector3.one * 1.1f;
    public float onEnterSizeDuration = 0.1f;

    [Space]
    public Vector3 onEnterRot = new Vector3(0,0,15);
    public float onEnterRotDuration = 0.1f;

    [Space]
    public Vector3 onExitSize = Vector3.one;
    public float onExitSizeDuration = 0.5f;

    [Space]
    public Vector3 onExitRot = new Vector3(0, 0, 0);
    public float onExitRotDuration = 0.5f;

    [Space]
    public Vector3 onClickSize = Vector3.one * 0.9f;
    public float onClickSizeDuration = 0.05f;

    Tween tween;
    Tween rotTween;

    private void OnDisable() {
        text.font = fontAsset;
    }

    private void OnEnable() {
        text.font = fontAsset;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (tween != null) { tween.Kill(true); }
        tween = transform.DOScale(onClickSize, onClickSizeDuration);

        if (rotTween != null) { rotTween.Kill(true); }
        rotTween = transform.DORotate(onExitRot, onClickSizeDuration);
    }


    public void OnPointerExit(PointerEventData eventData) {
        text.font = fontAsset;

        if(tween != null) { tween.Kill(true); }
        tween = transform.DOScale(onExitSize, onExitSizeDuration);

        if (rotTween != null) { rotTween.Kill(true); }
        rotTween = transform.DORotate(onExitRot, onExitRotDuration);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        text.font = enteredFontAsset;

        if (tween != null) { tween.Kill(true); }
        tween = transform.DOScale(onEnterSize, onEnterSizeDuration);

        if (rotTween != null) { rotTween.Kill(true); }
        var rand = Random.Range(-1, 1);
        if(rand == 0) { rand = 1; }
        rotTween = transform.DORotate(onEnterRot * rand, onEnterRotDuration);
    }
}
