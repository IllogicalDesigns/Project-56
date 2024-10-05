using System;
using Unity.Cinemachine;
using UnityEngine;

public class ImpulseOnHurt : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource impulseSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable() {
        var health = GetComponent<Health>();
        if(health != null) {
            health.DamageTaken += OnDamage;
        }
    }

    private void OnDisable() {
        var health = GetComponent<Health>();
        if (health != null) {
            health.DamageTaken -= OnDamage;
        }
    }

    private void OnDamage(Damage damage) {
        impulseSource.GenerateImpulse();
    }
}
