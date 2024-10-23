using System;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnEnable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter += OnLadderEnter;
        trigger.onTriggerExit += OnLadderExit;
    }

    private void OnDisable() {
        var trigger = GetComponent<Trigger>();
        trigger.onTriggerEnter -= OnLadderEnter;
        trigger.onTriggerExit -= OnLadderExit;
    }

    private void OnLadderExit(Collider other) {
        if(other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement)) {
            playerMovement.OnLadder = false;
        }
    }

    private void OnLadderEnter(Collider other) {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement)) {
            playerMovement.OnLadder = true;
        }
    }
}
