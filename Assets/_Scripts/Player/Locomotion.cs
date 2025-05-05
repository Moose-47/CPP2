using UnityEngine;

public class Locomotion : MonoBehaviour
{
    private CharacterController cc;
    private Animator anim;
    private Vector2 direction;
    private Vector3 velocity;
    private float curSpeed;

    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public PlayerAnimations playerAnimations;
    [HideInInspector] public LockOn lockOn;

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
        lockOn = GetComponentInChildren<LockOn>();

        float timeToJumpApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initJumpVelocity = -(gravity * timeToJumpApex);
    }

    void FixedUpdate()
    {
        if (playerState.IsDead) return;
        if (playerState.isAttacking) return;
        if (playerState.IsLockedOn)
        {
            HandleLockedOnRotation(direction, playerState.lockedOnTarget);
            HandleMovement();
        }
        else
        {
            HandleMovement();
            HandleRotation();
        }

        cc.Move(velocity);
        playerAnimations.UpdateAnimationVelocity(velocity, IsGrounded());
    }

    public void SetDirection(Vector2 dir) => direction = dir;
    public void SetJumpInput(bool isPressed) => isJumpPressed = isPressed;
    private void HandleMovement()
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

        // Animation Variables for movement
        Vector2 groundVel = new Vector2(velocity.x, velocity.z);
        float currentVel = anim.GetFloat("vel");
        float targetVel = groundVel.magnitude;
        smoothedVel = Mathf.Lerp(currentVel, targetVel, 10f * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.A))
        {
            smoothedVel = -Mathf.Clamp(targetVel, 0f, maxSpeed);
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (!Input.GetKey(KeyCode.A))
            {
                smoothedVel = Mathf.Abs(smoothedVel);
            }
        }

        if (!IsGrounded())
        {
            if (velocity.y > 0.1f)
                jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), 1f, 10f * Time.fixedDeltaTime);
            else
                jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), -1f, 0.5f * Time.fixedDeltaTime);
        }
        else
        {
            jumpVel = Mathf.Lerp(anim.GetFloat("jumpVel"), 0f, 10f * Time.fixedDeltaTime);
        }
    }

    private void HandleRotation()
    {
        if (playerState.IsLockedOn && playerState.lockedOnTarget != null)
        {
            // Locked-on strafing behavior
            Vector3 toTarget = (playerState.lockedOnTarget.position - transform.position).normalized;
            toTarget.y = 0f;

            Vector3 right = Vector3.Cross(Vector3.up, toTarget);
            Vector3 forward = toTarget;

            Vector3 moveDirection = (forward * direction.y + right * direction.x).normalized;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
        else if (direction != Vector2.zero)
        {
            // Free movement rotation based on camera
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * direction.y + camRight * direction.x;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    private float CheckJump()
    {
        if (isJumpPressed && velocity.y <= 0.01f)
            return initJumpVelocity;
        return velocity.y += gravity * Time.fixedDeltaTime;
    }
    public bool IsGrounded()
    {
        float checkDistance = 1f;
        Vector3 origin = transform.position;
        return Physics.SphereCast(origin, 0.2f, Vector3.down, out RaycastHit hit, checkDistance);
    }
    public void HandleLockedOnRotation(Vector2 input, Transform target)
    {
        if (target == null) return;

        // Direction to the target
        Vector3 toTarget = (target.position - transform.position).normalized;
        toTarget.y = 0f;

        // Strafing basis vectors
        Vector3 right = Vector3.Cross(Vector3.up, toTarget);
        Vector3 forward = toTarget;

        // Combine input into a movement direction
        Vector3 moveDirection = (forward * input.y + right * input.x).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        else
        {
            // No input, just look at the target
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }  
}