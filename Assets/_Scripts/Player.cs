using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float fallGravityMult = 2f;
    public float normalGravity = -9.81f;

    public Transform cameraTransform;

    public LayerMask groundLayer;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
       isGrounded = IsGrounded();
        if (!isGrounded)
        {
            Vector3 gravity = Vector3.up * normalGravity * fallGravityMult;
            rb.AddForce(gravity, ForceMode.Acceleration);
        } 
    }
    void FixedUpdate()
    {
        
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(hInput, 0f, vInput).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Get the camera's forward and right directions
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Flatten to ignore y-axis (vertical) movement
            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            // Calculate movement direction based on the camera's orientation
            Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;

            // Apply movement without rotating the player (keep character facing same direction)
            Vector3 moveAmount = speed * Time.fixedDeltaTime * moveDirection;
            rb.MovePosition(rb.position + moveAmount);

            // Optionally rotate the character to always face the same direction as the camera
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.5f, groundLayer);
    }
  
}