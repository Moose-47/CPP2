using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour, ProjectActions.IOverworldActions
{
    [HideInInspector] public Locomotion locomotion;
    [HideInInspector] public PlayerAnimations anim;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public ProjectActions input;
    [HideInInspector] public LockOn lockOn;
    [HideInInspector] public CameraController cameraController;
    private void OnEnable()
    {
        locomotion = GetComponentInChildren<Locomotion>();
        anim = GetComponent<PlayerAnimations>();
        playerState = GetComponent<PlayerState>();
        combat = GetComponentInChildren<PlayerCombat>();
        lockOn = GetComponentInChildren<LockOn>();
        cameraController = FindAnyObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.Log("camera controller missing");
        }

        input = new ProjectActions();
        input.Enable();
        input.Overworld.SetCallbacks(this);
    }

    private void OnDisable()
    {
        input.Disable();
        input.Overworld.RemoveCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        locomotion.SetDirection(context.canceled ? Vector2.zero : dir);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        locomotion.SetJumpInput(context.ReadValueAsButton());
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        playerState.TryDropWeapon(transform.forward);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && locomotion.IsGrounded())
        {
            combat.Attack();
        }
    }


    public void OnLockOn(InputAction.CallbackContext context) => lockOn.HandleLockOnInput();

    public void die()
    {
        playerState.Die();
        anim.deathAnimation();
    }
}