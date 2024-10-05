using System;
using UnityEngine;

public class soundOnHealth : MonoBehaviour {
    [SerializeField] AudioClip hurt;
    [SerializeField] AudioClip death;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    private void OnEnable() {
        var health = GetComponent<Health>();
        if (health != null) {
            health.DamageTaken += OnDamage;
            health.DamageTaken += OnDeath;
        }
    }

    private void OnDisable() {
        var health = GetComponent<Health>();
        if (health != null) {
            health.DamageTaken -= OnDamage;
            health.DamageTaken -= OnDeath;
        }
    }

    private void OnDamage(Damage damage) {
        AudioSource.PlayClipAtPoint(hurt, transform.position);
    }

    private void OnDeath(Damage damage) {
        AudioSource.PlayClipAtPoint(death, transform.position);
    }
}

