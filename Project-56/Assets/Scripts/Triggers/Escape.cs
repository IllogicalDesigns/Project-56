using System;
using UnityEngine;

public class Escape : MonoBehaviour
{
    private void OnEnable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter += OnTriggered;
    }

    private void OnDisable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter -= OnTriggered;
    }

    private void OnTriggered(Collider other) {
        FindAnyObjectByType<GameManager>()?.Escaped();
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
