using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DwarfController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float normalHeight = 1.0f;
    public float crouchHeight = 0.5f;
    public float crouchTransitionSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = normalHeight;
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small constant downward force to snap to ground
        }

        // Inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determine Speed
        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = sprintSpeed;

        // Move
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : normalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        
        // Adjust center to keep the model grounded if needed
        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;
    }
}
