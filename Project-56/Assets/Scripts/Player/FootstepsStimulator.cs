using System.Collections;
using UnityEngine;

public class FootstepsStimulator : MonoBehaviour {
    Vector3 lastPos = Vector3.zero;
    private float minStepDist = 1.5f;

    PlayerMovement player;

    Stimulus stimulus;
    [SerializeField] float footStepAwareness = 3f;
    [SerializeField] float jumpAwareness = 4f;
    [SerializeField] float landAwareness = 4f;
    [SerializeField] int priority = 2;

    bool canStimulate;
    float waitBeforeStimulation = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start() {
        player = GameManager.player;
        lastPos = transform.position;

        GameObject stimObj = new GameObject("Footstep stimulus");
        stimulus = stimObj.AddComponent<Stimulus>();
        stimulus.priority = priority;

        yield return new WaitForSeconds(waitBeforeStimulation);
        canStimulate = true;
    }

    private void OnEnable() {
        var player = FindAnyObjectByType<PlayerMovement>();
        if(player != null) {
            player.jump += OnJump;
            player.land += OnLand;
        }
    }

    private void OnDisable() {
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) {
            player.jump += OnJump;
            player.land += OnLand;
        }
    }

    void OnJump() {
        if(!canStimulate) return;

        lastPos = transform.position;
        UpdateStimulus(jumpAwareness);
    }

    void OnLand() {
        if (!canStimulate) return;

        lastPos = transform.position;
        UpdateStimulus(landAwareness);
    }

    // Update is called once per frame
    void Update() {
        if (!canStimulate) return;

        if (Vector3.Distance(lastPos, transform.position) > minStepDist) {
            lastPos = transform.position;

            if (player.crouched) return;
            if (!player.isGrounded) return;
            UpdateStimulus(footStepAwareness);
        }
    }

    private void UpdateStimulus(float awareness) {
        stimulus.transform.position = transform.position;
        stimulus.UpdateStim(awareness);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lastPos, 0.5f);
    }
}

