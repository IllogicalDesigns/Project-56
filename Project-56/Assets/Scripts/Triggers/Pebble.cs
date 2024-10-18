using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Pebble : MonoBehaviour {
    [SerializeField] AudioClip pickup;

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

    // Update is called once per frame
    void Update() {

    }
}

