using UnityEngine;
using DG.Tweening;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField] Vector3 rotation = new Vector3 (0f, 10f, 0f);
    [SerializeField] float duration = 10f;
    [SerializeField] Ease ease = Ease.InOutElastic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOBlendableLocalRotateBy(rotation, duration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
