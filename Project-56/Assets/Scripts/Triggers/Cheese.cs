using DG.Tweening;
using UnityEngine;

public class Cheese : MonoBehaviour
{
    [SerializeField] AudioClip pickup;
    [SerializeField] float yHeight = 1f;
    [SerializeField] float yDuration = 1f;
    [SerializeField] Ease ease = Ease.InOutSine;

    [SerializeField] Vector3 rotateVec = Vector3.up;
    [SerializeField] float rotDuration = 1f;

    private void OnEnable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter += OnPickup;
    }

    private void OnDisable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter -= OnPickup;
    }

    private void OnPickup(Collider other) {
        var gm = FindAnyObjectByType<GameManager>();
        gm.CheeseCollected();
        if(pickup != null) AudioSource.PlayClipAtPoint(pickup, transform.position);
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOLocalMoveY(transform.position.y + yHeight, (yDuration + Random.Range(0.01f, 1.01f))).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        transform.DOBlendableLocalRotateBy(rotateVec, rotDuration).SetLoops(-1, LoopType.Incremental);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
