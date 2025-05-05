using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachine
{
    /// <summary>
    /// Example player context class for the state machine.
    /// </summary>
    [Serializable]
    public class ExampleContext : ReactiveContext<ExampleContext>
    {
        // Public configuration properties
        public float MoveSpeed = 5f;
        public float MaxJumpHeight = 2f;
        public float TimeToJumpApex = 0.4f;
        public float CoyoteTime = 0.1f;
        public float JumpBufferTime = 0.2f;
        public float WallSlideSpeed = 1f;

        // References
        public Rigidbody2D Rigidbody { get; set; }
        public Animator Animator { get; set; }
        public Transform Transform { get; set; }
        
        // Input actions
        public InputAction JumpAction { get; set; }
        public InputAction AttackAction { get; set; }
        
        // Calculate gravity and jump velocity based on height and time
        public float Gravity => -(2 * MaxJumpHeight) / (TimeToJumpApex * TimeToJumpApex);
        public float JumpVelocity => Mathf.Abs(Gravity) * TimeToJumpApex;

        #region Reactive Properties
        // Movement properties
        private Vector2 moveInput;
        private bool isGrounded;
        private bool isJumping;
        private bool isRunning;

        // Combat properties
        private bool isAttacking;
        private int comboCount;
        private float attackTimer;
        private bool isStunned;
        private int health;

        // Environment interaction
        private bool isNearLadder;
        private bool isNearWall;
        private bool isClimbing;
        #endregion

        #region Reactive Property Accessors
        /// <summary>
        /// The movement input vector.
        /// </summary>
        public Vector2 MoveInput
        {
            get => moveInput;
            set => SetProperty(ref moveInput, value, nameof(MoveInput));
        }

        /// <summary>
        /// Whether the player is grounded.
        /// </summary>
        public bool IsGrounded
        {
            get => isGrounded;
            set => SetProperty(ref isGrounded, value, nameof(IsGrounded));
        }

        /// <summary>
        /// Whether the player is jumping.
        /// </summary>
        public bool IsJumping
        {
            get => isJumping;
            set => SetProperty(ref isJumping, value, nameof(IsJumping));
        }

        /// <summary>
        /// Whether the player is running.
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value, nameof(IsRunning));
        }

        /// <summary>
        /// Whether the player is attacking.
        /// </summary>
        public bool IsAttacking
        {
            get => isAttacking;
            set => SetProperty(ref isAttacking, value, nameof(IsAttacking));
        }

        /// <summary>
        /// The current combo count for attacks.
        /// </summary>
        public int ComboCount
        {
            get => comboCount;
            set => SetProperty(ref comboCount, value, nameof(ComboCount));
        }

        /// <summary>
        /// Timer for attack-related mechanics.
        /// </summary>
        public float AttackTimer
        {
            get => attackTimer;
            set => SetProperty(ref attackTimer, value, nameof(AttackTimer));
        }

        /// <summary>
        /// Whether the player is stunned.
        /// </summary>
        public bool IsStunned
        {
            get => isStunned;
            set => SetProperty(ref isStunned, value, nameof(IsStunned));
        }

        /// <summary>
        /// The player's health.
        /// </summary>
        public int Health
        {
            get => health;
            set => SetProperty(ref health, value, nameof(Health));
        }

        /// <summary>
        /// Whether the player is near a ladder.
        /// </summary>
        public bool IsNearLadder
        {
            get => isNearLadder;
            set => SetProperty(ref isNearLadder, value, nameof(IsNearLadder));
        }

        /// <summary>
        /// Whether the player is near a wall.
        /// </summary>
        public bool IsNearWall
        {
            get => isNearWall;
            set => SetProperty(ref isNearWall, value, nameof(IsNearWall));
        }

        /// <summary>
        /// Whether the player is climbing.
        /// </summary>
        public bool IsClimbing
        {
            get => isClimbing;
            set => SetProperty(ref isClimbing, value, nameof(IsClimbing));
        }
        #endregion
    }
}
