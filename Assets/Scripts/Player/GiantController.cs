using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GiantController : MonoBehaviour
{
    [Header("Network/Local Settings")]
    public bool isLocalPlayer = false;

    [Header("Movement Settings (Giant)")]
    public float walkSpeed = 10.0f; 
    public float sprintSpeed = 18.0f;
    public float gravity = -20f; 

    [Header("Carry System")]
    public Transform holdPosition;
    public float interactionRange = 15f; 
    private InteractableItem currentItem;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
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

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
                Camera mainCam = GetComponentInChildren<Camera>();
                if (mainCam != null)
                {
                    if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, interactionRange))
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
