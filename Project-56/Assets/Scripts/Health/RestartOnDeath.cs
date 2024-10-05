using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnDeath : MonoBehaviour {
    private void OnEnable() {
        var hp = GetComponent<Health>();
        hp.death += OnDeath;
    }

    private void OnDisable() {
        var hp = GetComponent<Health>();
        hp.death -= OnDeath;
    }

    private void OnDeath(Damage damage) {
        Debug.Log("Restarting the player has died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

