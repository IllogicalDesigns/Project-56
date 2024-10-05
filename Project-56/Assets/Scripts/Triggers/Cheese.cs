using System;
using UnityEngine;

public class Cheese : MonoBehaviour
{
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
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
