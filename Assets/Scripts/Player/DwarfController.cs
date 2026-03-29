using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DwarfController : MonoBehaviour
{
    [Header("Network/Local Settings")]
    public bool isLocalPlayer = false;

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float carrySpeedMultiplier = 0.4f; 
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float normalHeight = 1.0f;
    public float crouchHeight = 0.5f;
    public float crouchTransitionSpeed = 10f;

    [Header("Camera System")]
    public Camera firstPersonCam;
    public Camera thirdPersonCam;
    
    [Header("Tension System")]
    public Renderer headRenderer; 
    public Transform giantTransform;
    public float tensionDistance = 25f; 

    [Header("Carry System")]
    public Transform holdPosition;
    public float interactionRange = 3f;
    private InteractableItem currentItem;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private Color originalHeadColor = Color.green;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = normalHeight;
        
        if (headRenderer != null)
        {
            originalHeadColor = headRenderer.material.color;
        }

        if (firstPersonCam != null) firstPersonCam.enabled = false;
        if (thirdPersonCam != null) thirdPersonCam.enabled = true; 
        
        if (giantTransform == null)
        {
            var giant = FindFirstObjectByType<GiantController>();
            if (giant != null) giantTransform = giant.transform;
        }
    }

    void Update()
    {
        // Tension system should still work even if we are not the local player
        // (So the Giant can see the dwarf's head turn red)
        HandleTension();

        if (!isLocalPlayer) return;

        HandleMovement();
        HandleCrouch();
        HandleCameraSwap();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = sprintSpeed;

        if (currentItem != null)
        {
            currentSpeed *= carrySpeedMultiplier;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching && currentItem == null)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

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
        
        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;
    }

    private void HandleCameraSwap()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (firstPersonCam != null && thirdPersonCam != null)
            {
                bool isFP = firstPersonCam.enabled;
                firstPersonCam.enabled = !isFP;
                thirdPersonCam.enabled = isFP;
            }
        }
    }

    private void HandleTension()
    {
        if (giantTransform == null || headRenderer == null) return;

        float distance = Vector3.Distance(transform.position, giantTransform.position);
        
        // Mathematical Lerp logic for smooth tension color transition
        // distanceFactor = 0 when distance is fully safe (>= tensionDistance)
        // distanceFactor = 1 when distance is 0 (dead center overlap)
        float distanceFactor = 1f - Mathf.Clamp01(distance / tensionDistance);

        // Smooth color lerp from green to red based on distance
        headRenderer.material.color = Color.Lerp(originalHeadColor, Color.red, distanceFactor);
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentItem != null)
            {
                currentItem.Drop();
                currentItem = null;
            }
            else
            {
                Camera activeCam = null;
                if (firstPersonCam != null && firstPersonCam.enabled) activeCam = firstPersonCam;
                else if (thirdPersonCam != null && thirdPersonCam.enabled) activeCam = thirdPersonCam;

                if (activeCam != null)
                {
                    if (Physics.Raycast(activeCam.transform.position, activeCam.transform.forward, out RaycastHit hit, interactionRange))
                    {
                        InteractableItem item = hit.collider.GetComponent<InteractableItem>();
                        if (item != null && !item.isBeingCarried)
                        {
                            currentItem = item;
                            currentItem.PickUp(holdPosition);
                        }
                    }
                }
            }
        }
    }
}
