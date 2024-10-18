using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Cheese : MonoBehaviour
{
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
        var gm = FindAnyObjectByType<GameManager>();
        gm.CheeseCollected();
        if(pickup != null) AudioSource.PlayClipAtPoint(pickup, transform.position);
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }
}
