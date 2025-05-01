using UnityEngine;

public class Locomotion : MonoBehaviour
{
    private CharacterController cc;
    private Animator anim;
    private Vector2 direction;
    private Vector3 velocity;
    private float curSpeed;

    [Header("References")]
    public PlayerState playerState;
    public PlayerAnimations playerAnimations;

    [Header("Settings")]
    public float maxSpeed = 15f;
    public float moveAccel = 0.2f;
    public float deceleration = 5f;
    public float jumpHeight = 0.1f;
    public float jumpTime = 0.7f;

    public float jumpVel = 0f;
    public float smoothedVel = 0f;

    private float gravity;
    private float initJumpVelocity;
    private bool isJumpPressed;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerState = gameObject.GetComponent<PlayerState>();
        playerAnimations = gameObject.GetComponent<PlayerAnimations>();

        float timeToJumpApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initJumpVelocity = -(gravity * timeToJumpApex);
    }

    void FixedUpdate()
    {
        if (playerState.IsDead) return;

        UpdateMovement();
        cc.Move(velocity);
        playerAnimations.UpdateAnimationVelocity(velocity, IsGrounded());
    }

    public void SetDirection(Vector2 dir) => direction = dir;
    public void SetJumpInput(bool isPressed) => isJumpPressed = isPressed;

    private void UpdateMovement()
    {
        // Get camera vectors
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Flatten camera vectors
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Get intended movement direction
        Vector3 moveDirection = camForward * direction.y + camRight * direction.x;

        // Speed control
        if (direction != Vector2.zero)
        {
            // Accelerate
            curSpeed += moveAccel * Time.fixedDeltaTime;
            curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);

            // Apply velocity in direction
            velocity.x = moveDirection.x * curSpeed;
            velocity.z = moveDirection.z * curSpeed;

            if (!playerState.IsLockedOn)
            {
                // Rotate player toward move direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
        else
        {
            // Decelerate
            curSpeed -= deceleration * Time.fixedDeltaTime;
            curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);

            // Smoothly reduce horizontal velocity
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

        // Gravity & Jump
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = CheckJump();
        }

        if (direction == Vector2.zero && new Vector2(velocity.x, velocity.z).magnitude < 0.1f)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        #region Animation Values and Variables
        //Handling idle/walk/run animation variable
        Vector2 groundVel = new Vector2(velocity.x, velocity.z);
        float currentVel = anim.GetFloat("vel");
        float targetVel = groundVel.magnitude;
        smoothedVel = Mathf.Lerp(currentVel, targetVel, 10f * Time.fixedDeltaTime); // Adjust float value up for snappiness

        //Handling Jump and fall animation variable
        if (!IsGrounded())
            if (velocity.y > 0f)
            {
                jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), 1f, 10f * Time.fixedDeltaTime);
            }
            else
            {
                jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), -1f, 1.5f * Time.fixedDeltaTime);
            }
        else
        {
            jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), 0f, 10f * Time.fixedDeltaTime);
        }
        #endregion
    }
    private float CheckJump()
    {
        if (isJumpPressed && velocity.y <= 0.01f)
            return initJumpVelocity;
        return -cc.minMoveDistance;
    }
    public bool IsGrounded()
    {
        float checkDistance = 1f;
        Vector3 origin = transform.position;
        return Physics.SphereCast(origin, 0.3f, Vector3.down, out RaycastHit hit, checkDistance);
    }
}