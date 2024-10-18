using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public const string verticalAxisName = "Vertical";
    public const string horizontalAxisName = "Horizontal";
    public const string mouseXAxisName = "Mouse X";
    public const string mouseYAxisName = "Mouse Y";

    public const KeyCode sprintKeyName = KeyCode.LeftShift;
    public const KeyCode crouchKeyName = KeyCode.LeftControl;
    public const KeyCode altCrouchKeyName = KeyCode.C;
    public const KeyCode jumpKeyName = KeyCode.Space;

    public bool canMove = true;  //Used to prevent movement for instance on pause

    CharacterController characterController;

    [Header("Speeds")]
    [SerializeField] float playerMoveSpeed = 3.0f;
    [SerializeField] float playerCrouchSpeed = 1.0f;
    [SerializeField] float playerSprintSpeed = 6.0f;
    public float rotationSpeed = 64.0f;
    public bool sprinting;

    [Header("Crouches")]
    [SerializeField] float crouchedHeight = 0.25f;
    float originalHeight;
    public bool crouched;

    [Header("Jumps")]
    [SerializeField] float jumpHeight = 0.25f;
    [SerializeField] float coyoteTime = 1f;
    [SerializeField] bool doubleJumpAllowed = true;
    float jumpMagicNumber = -3.0f;  //just given out by unity
    float coyoteTimer;
    bool doubleJump;

    [Header("Cameras")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float minXRotation = -45, maxXRotation = 45;

    [Header("Misc")]
    [SerializeField] float gravityValue = -9.81f;
    [SerializeField] float pullDownRayLength = 1.75f;

    public bool isGrounded;
    bool wasGrounded;
    Vector3 playerVelocity;
    Vector3 moveDirection;

    public delegate void OnJump();
    public event OnJump jump;

    public delegate void OnLand();
    public event OnLand land;


    void Start() {
        // Get reference to the Character Controller
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
    }

    // Update is called once per frame
    void Update() {
        if (!canMove) { return; }

        coyoteTimer -= Time.deltaTime;

        HandleGrounding();

        if (Input.GetKeyDown(crouchKeyName) || Input.GetKeyDown(altCrouchKeyName)) 
            crouched = !crouched;

        Vector2 input = new Vector2(Input.GetAxis(verticalAxisName), Input.GetAxis(horizontalAxisName));

        if (input.magnitude > 1)
            input.Normalize();

        moveDirection = transform.forward * input.x;
        moveDirection += transform.right * input.y;

        sprinting = Input.GetKey(sprintKeyName);
        if (sprinting) crouched = false;
        if (crouched) sprinting = false;

        HandleRotation();
        HandleJump();
        HandleCrouch();
    }

    private void HandleJump() {
        // Changes the height position of the player..
        if (Input.GetKeyDown(jumpKeyName) && !(isGrounded || coyoteTimer > 0) && doubleJump) {
            Jump();
            doubleJump = false;
        }
        else if (Input.GetKeyDown(jumpKeyName) && (isGrounded || coyoteTimer > 0)) {
            playerVelocity.y = 0f;
            Jump();
        }
    }

    private void Jump() {
        crouched = false;
        playerVelocity.y += Mathf.Sqrt((jumpHeight * transform.localScale.magnitude) * jumpMagicNumber * (gravityValue * transform.localScale.magnitude));
        coyoteTimer = 0f;
        jump?.Invoke();
    }

    private void HandleRotation() {
        transform.Rotate(0, Input.GetAxis(mouseXAxisName) * rotationSpeed * Time.deltaTime, 0);
        RotateCameraUpDwn();
    }

    private void RotateCameraUpDwn() {
        // Get the current rotation
        Vector3 currentRotation = cameraTransform.localEulerAngles;
        float currentXRotation = currentRotation.x;

        // Adjust for 360 to -180/180 range issues (Unity returns rotation from 0 to 360)
        if (currentXRotation > 180) currentXRotation -= 360;

        // Calculate the new rotation
        float newXRotation = currentXRotation + Input.GetAxis(mouseYAxisName) * -rotationSpeed * Time.deltaTime;

        // Clamp the new rotation between minX and maxX
        newXRotation = Mathf.Clamp(newXRotation, minXRotation, maxXRotation);

        // Apply the clamped rotation back to the camera
        cameraTransform.localEulerAngles = new Vector3(newXRotation, currentRotation.y, currentRotation.z);
    }

    void HandleGrounding() {
        isGrounded = characterController.isGrounded;

        if (!wasGrounded && isGrounded) {
            land?.Invoke();
        }

        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
            coyoteTimer = coyoteTime;
            ResetDoubleJump();
        }

        wasGrounded = isGrounded;
    }

    private void ResetDoubleJump() {
        if (doubleJumpAllowed) {
            doubleJump = true;
        }
        else {
            doubleJump = false;
        }
    }

    private void FixedUpdate() {
        if (!canMove) { return; }

        HandleCrouch();

        var speed = GetSpeed();
        characterController.Move(moveDirection * speed * Time.deltaTime);

        HandleGravity();
        PullCharacterToTheGround();
    }

    private float GetSpeed() {
        if(!isGrounded) return playerSprintSpeed;

        var speed = playerMoveSpeed;

        if (crouched) {
            speed = playerCrouchSpeed;
        }
        else if (sprinting) {
            speed = playerSprintSpeed;
        }

        return speed;
    }

    private void HandleGravity() {
        playerVelocity.y += (gravityValue * transform.localScale.magnitude) * Time.deltaTime;
        var onlyY = new Vector3(0, playerVelocity.y, 0);
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleCrouch() {
        characterController.height = (crouched ? crouchedHeight : originalHeight);
    }

    public void SetMoveAllowance(bool allowedToMove) {
        canMove = allowedToMove;
    }

    void PullCharacterToTheGround() {
        if (Input.GetKeyDown(jumpKeyName)) return;
        if (coyoteTimer <= 0) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, pullDownRayLength)) {
            float groundDistance = hit.distance;
            if (groundDistance > characterController.skinWidth) {
                // Pull the character controller downward to stick to the ground
                characterController.Move(Vector3.down * groundDistance * 0.5f);
            }
        }
    }
}
