using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PickupFloat : MonoBehaviour
{
    [SerializeField] float yHeight = 1f;
    [SerializeField] float yDuration = 1f;
    [SerializeField] Ease ease = Ease.InOutSine;

    [SerializeField] Vector3 rotateVec = Vector3.up;
    [SerializeField] Vector3 rotateVec2 = Vector3.up;
    [SerializeField] float rotDuration = 1f;
    [SerializeField] float rotDuration2 = 1f;
    [SerializeField] Ease ease2 = Ease.Linear;
    [SerializeField] Ease ease3 = Ease.InOutSine;

    IEnumerator Start() {
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        transform.DOMoveY(transform.position.y + yHeight, (yDuration + Random.Range(0.01f, 1.01f))).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        transform.DOBlendableLocalRotateBy(rotateVec, rotDuration).SetLoops(-1, LoopType.Incremental).SetEase(ease2);
        transform.DOBlendableLocalRotateBy(rotateVec2, rotDuration2).SetLoops(-1, LoopType.Yoyo).SetEase(ease3);
    }
}
