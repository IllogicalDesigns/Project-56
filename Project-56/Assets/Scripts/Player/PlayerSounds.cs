using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    Vector3 lastPos = Vector3.zero;
    private float minStepDist = 1.5f;
    PlayerMovement player;
    [SerializeField] AudioClip step;
    [SerializeField] AudioClip crouchStep;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip land;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        player = GameManager.player;
        lastPos = transform.position;
    }

    private void OnEnable() {
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) {
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
        lastPos = transform.position;
        AudioSource.PlayClipAtPoint(jump, transform.position);
    }

    void OnLand() {
        lastPos = transform.position;
        AudioSource.PlayClipAtPoint(land, transform.position);
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(lastPos, transform.position) > minStepDist) {
            lastPos = transform.position;

            if (!player.isGrounded) return;
            
            if(player.crouched) {
                AudioSource.PlayClipAtPoint(crouchStep, transform.position);
            } else {
                AudioSource.PlayClipAtPoint(step, transform.position);
            }
        }
    }
}


