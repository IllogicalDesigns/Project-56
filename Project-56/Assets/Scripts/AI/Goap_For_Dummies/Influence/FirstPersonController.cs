using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    
    float oldHeight = 2f;
    [SerializeField] float newHeight = 1f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        oldHeight = controller.height;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftControl))
            controller.height = newHeight;
        else 
            controller.height = oldHeight;
        
        // Rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(0f, mouseX, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float moveX = Input.GetAxis("Horizontal") * movementSpeed;
        float moveZ = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 movement = transform.right * moveX + transform.forward * moveZ;

        // Apply gravity
        verticalVelocity -= gravity * Time.deltaTime;

        // Jump
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;
        }

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }
}