using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, ProjectActions.IOverworldActions
{
    //private Rigidbody rb;
    CharacterController cc;
    ProjectActions input;

    #region Character Controller movement variables
    [Header("Movement Variables")]
    [SerializeField] private float initSpeed = 5.0f;
    [SerializeField] private float maxSpeed = 15.0f;
    [SerializeField] private float moveAccel = 0.2f;
    private float curSpeed = 5.0f;

    //Character Movement Variables
    Vector2 direction;
    Vector3 velocity;

    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 0.1f;
    [SerializeField] private float jumpTime = 0.7f;

    //values calculated using our jump height
    [SerializeField] private float timeToJumpApex; //JumptTime / 2
    [SerializeField] private float initJumpVelocity;

    private bool isJumpPressed = false;

    //calculated based on our jump values - this is the Y velocity that we will apply
    [SerializeField] private float gravity;
    #endregion
   
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();

        #region CC jump variable values
        timeToJumpApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initJumpVelocity = -(gravity * timeToJumpApex);
        #endregion
    }

    void FixedUpdate()
    {
        UpdateCharacterVelocity();

        cc.Move(velocity);
    }   
    
    private void UpdateCharacterVelocity()
    {
        if (direction == Vector2.zero) curSpeed = initSpeed;

        velocity.x = direction.x * curSpeed;
        velocity.z = direction.y * curSpeed;

        curSpeed += moveAccel * Time.fixedDeltaTime;

        if (!cc.isGrounded) velocity.y += gravity * Time.fixedDeltaTime;
        else velocity.y = CheckJump();
    }

    private float CheckJump()
    {
        if (isJumpPressed) return initJumpVelocity;
        else return -cc.minMoveDistance;
    }
    void OnEnable()
    {
        input = new ProjectActions();
        input.Enable();
        input.Overworld.SetCallbacks(this);
    }

    void OnDisable()
    {
        input.Disable();
        input.Overworld.RemoveCallbacks(this);
    }

    #region Input Functions
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) direction = context.ReadValue<Vector2>();
        if (context.canceled) direction = Vector2.zero;
    }
    public void OnJump(InputAction.CallbackContext context) => isJumpPressed = context.ReadValueAsButton();

    #endregion 
    #region Rigid Body Movement variables
    //[Header("rigid body movement")]
    //public float speed = 5f;
    //public float rotationSpeed = 10f;
    //public float jumpForce = 5f;
    //public float fallGravityMult = 2f;
    //public float normalGravity = -9.81f;

    //public Transform cameraTransform;

    //public LayerMask groundLayer;

    //private bool isGrounded;
    #endregion
    #region Rigid Body Movement and jump
    //private void Update()
    //{
    //    #region Rigid Body jump/GC
    //    isGrounded = IsGrounded();
    //    if (!isGrounded)
    //    {
    //        Vector3 gravity = Vector3.up * normalGravity * fallGravityMult;
    //        rb.AddForce(gravity, ForceMode.Acceleration);
    //    }
        
    //}
    //void FixedUpdate()
    //{

    //    float hInput = Input.GetAxis("Horizontal");
    //    float vInput = Input.GetAxis("Vertical");

    //    Vector3 inputDirection = new Vector3(hInput, 0f, vInput).normalized;

    //    if (inputDirection.magnitude >= 0.1f)
    //    {
    //        //get the camera's forward and right directions
    //        Vector3 camForward = cameraTransform.forward;
    //        Vector3 camRight = cameraTransform.right;

    //        //flatten to ignore y-axis (vertical) movement
    //        camForward.y = 0f;
    //        camRight.y = 0f;

    //        camForward.Normalize();
    //        camRight.Normalize();

    //        //calculate movement direction based on the camera's orientation
    //        Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;

    //        //apply movement without rotating the player (keep character facing same direction)
    //        Vector3 moveAmount = speed * Time.fixedDeltaTime * moveDirection;
    //        rb.MovePosition(rb.position + moveAmount);

    //        //rotate the character to always face the same direction as the camera
    //        Quaternion targetRotation = Quaternion.LookRotation(camForward);
    //        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    //    }

    //    if (isGrounded && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Jump();
    //    }
    //}

    //void Jump()
    //{
    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //}

    //bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, 1.5f, groundLayer);
    //}
    #endregion
}