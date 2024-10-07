using DG.Tweening;
using UnityEngine;

public class Pebble : MonoBehaviour {
    [SerializeField] AudioClip pickup;
    [SerializeField] float yHeight = 0.1f;
    [SerializeField] float yDuration = 1f;
    [SerializeField] Ease ease = Ease.InOutSine;

    [SerializeField] Vector3 rotateVec = new Vector3(0, 50, 0);
    [SerializeField] Vector3 rotateVec2 = new Vector3(0,0,-20f);
    [SerializeField] float rotDuration = 1f;
    [SerializeField] float rotDuration2 = 1f;
    [SerializeField] Ease ease2 = Ease.Linear;
    [SerializeField] Ease ease3 = Ease.InOutSine;

    private void OnEnable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter += OnPickup;
    }

    private void OnDisable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter -= OnPickup;
    }

    private void OnPickup(Collider other) {
        FindAnyObjectByType<Throw>().ammo++;
        AudioSource.PlayClipAtPoint(pickup, transform.position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        transform.DOMoveY(transform.position.y + yHeight, (yDuration + Random.Range(0.01f, 1.01f))).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        transform.DOBlendableLocalRotateBy(rotateVec, rotDuration).SetLoops(-1, LoopType.Incremental).SetEase(ease2);
        transform.DOBlendableLocalRotateBy(rotateVec2, rotDuration2).SetLoops(-1, LoopType.Yoyo).SetEase(ease3);
    }

    // Update is called once per frame
    void Update() {

    }
}

